using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Controllers
{
    public class GioHangController : Controller
    {
        private readonly WebBanSachContext _context;

        public GioHangController(WebBanSachContext context)
        {
            _context = context;
        }

        // Hiển thị giỏ hàng của user
        public IActionResult Index()
        {
            var maND = HttpContext.Session.GetInt32("MaND");
            if (maND == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == maND)
                .ToList();

            ViewBag.Total = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan);

            return View(gioHang);
        }

        // Thêm sách vào giỏ
        [HttpPost]
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

        // Cập nhật số lượng
        [HttpPost]
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

        // Xóa khỏi giỏ
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
    }
}
