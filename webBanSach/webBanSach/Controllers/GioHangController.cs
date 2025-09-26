using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using System;
using System.Linq;

namespace webBanSach.Controllers
{
    public class GioHangController : Controller
    {
        private readonly WebBanSachContext _context;

        public GioHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // 📦 Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("MaND");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "GioHang") });
            }

            var gioHang = _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == userId)
                .ToList();

            ViewBag.Total = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan);
            return View(gioHang);
        }

        // ➕ Thêm sách vào giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int maSach, int soLuong = 1)
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItem = _context.GioHangs
                .FirstOrDefault(g => g.MaND == maND && g.MaSach == maSach);

            if (cartItem == null)
            {
                var newItem = new GioHang
                {
                    MaND = maND.Value,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(newItem);
            }
            else
            {
                cartItem.SoLuong += soLuong;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // 🔄 Cập nhật số lượng sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int maSach, int soLuong)
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login", "Account");

            var item = _context.GioHangs.FirstOrDefault(g => g.MaND == maND && g.MaSach == maSach);
            if (item != null)
            {
                item.SoLuong = soLuong > 0 ? soLuong : 1;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ❌ Xóa sản phẩm khỏi giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int maSach)
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login", "Account");

            var item = _context.GioHangs.FirstOrDefault(g => g.MaND == maND && g.MaSach == maSach);
            if (item != null)
            {
                _context.GioHangs.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // 💳 GET: Trang xác nhận thanh toán
        [HttpGet]
        public IActionResult ThanhToan()
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ThanhToan", "GioHang") });
            }

            var gioHang = _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == maND)
                .ToList();

            if (!gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index");
            }

            ViewBag.Total = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan);
            return View(gioHang); // => Views/GioHang/ThanhToan.cshtml
        }

        // 🛒 POST: "Mua ngay" -> thêm sản phẩm rồi chuyển đến trang ThanhToan
        [HttpPost]
        public IActionResult ThanhToan(int maSach, int soLuong = 1)
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login", "Account");

            var cartItem = _context.GioHangs
                .FirstOrDefault(g => g.MaND == maND && g.MaSach == maSach);

            if (cartItem == null)
            {
                _context.GioHangs.Add(new GioHang
                {
                    MaND = maND.Value,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                });
            }
            else
            {
                cartItem.SoLuong += soLuong;
            }

            _context.SaveChanges();

            // Sau khi thêm sản phẩm → mở trang ThanhToan (GET)
            return RedirectToAction("ThanhToan");
        }

        // ✅ POST: Xử lý thanh toán, tạo đơn hàng và chi tiết đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToanConfirm()
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null) return RedirectToAction("Login", "Account");

            var gioHang = _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == maND)
                .ToList();

            if (!gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "GioHang");
            }

            // 🧾 Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaND = maND.Value,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xử lý",
                TongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan)
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            // 📦 Tạo chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var chiTiet = new CT_DonHang
                {
                    MaDH = donHang.MaDH,
                    MaSach = item.MaSach,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSachNavigation.GiaBan
                };
                _context.CT_DonHangs.Add(chiTiet);
            }

            // 🗑️ Xóa giỏ hàng sau khi đặt hàng
            _context.GioHangs.RemoveRange(gioHang);
            _context.SaveChanges();

            TempData["Success"] = "✅ Thanh toán thành công! Đơn hàng của bạn đã được ghi nhận.";
            return RedirectToAction("Index", "DonHang");
        }

        // 🛒 Mua ngay 1 sản phẩm -> thêm vào giỏ và chuyển đến trang Thanh Toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyNow(int maSach, int soLuong = 1)
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Sach") });

            var cartItem = _context.GioHangs.FirstOrDefault(g => g.MaND == maND && g.MaSach == maSach);

            if (cartItem == null)
            {
                var newItem = new GioHang
                {
                    MaND = maND.Value,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(newItem);
            }
            else
            {
                cartItem.SoLuong += soLuong;
            }

            _context.SaveChanges();

            // 👉 Sau khi thêm thì chuyển thẳng đến trang ThanhToan
            return RedirectToAction("ThanhToan", "GioHang");
        }

    }
}
