using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

namespace webBanSach.Controllers
{
    public class GioHangsController : Controller
    {
        private readonly WebBanSachContext _context;

        public GioHangsController(WebBanSachContext context)
        {
            _context = context;
        }

        // 🛒 Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaSachNavigation)
                .Where(g => g.MaND == userId)
                .ToListAsync();

            return View(gioHang);
        }

        // 🛒 Thêm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int maSach, int soLuong = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHangItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (gioHangItem != null)
            {
                gioHangItem.SoLuong += soLuong;
            }
            else
            {
                gioHangItem = new GioHang
                {
                    MaND = userId.Value,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(gioHangItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 🛒 Mua ngay (chuyển đến giỏ hàng sau khi thêm sách)
        [HttpPost]
        public async Task<IActionResult> MuaNgay(int maSach, int soLuong = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHangItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (gioHangItem == null)
            {
                gioHangItem = new GioHang
                {
                    MaND = userId.Value,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(gioHangItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // 🛒 Cập nhật số lượng
        [HttpPost]
        public async Task<IActionResult> CapNhat(int maSach, int soLuong)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHangItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (gioHangItem != null)
            {
                if (soLuong > 0)
                {
                    gioHangItem.SoLuong = soLuong;
                }
                else
                {
                    _context.GioHangs.Remove(gioHangItem);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // 🛒 Xóa khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> Xoa(int maSach)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHangItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaND == userId && g.MaSach == maSach);

            if (gioHangItem != null)
            {
                _context.GioHangs.Remove(gioHangItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
