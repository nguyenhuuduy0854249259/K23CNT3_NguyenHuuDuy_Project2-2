using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;

public class DonHangController : Controller
{
    private readonly WebBanSachContext _context;

    public DonHangController(WebBanSachContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Xác nhận đặt hàng
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ThanhToanConfirm(string HoTen, string Email, string DiaChi, string PaymentMethod)
    {
        // Kiểm tra đăng nhập (dùng MaND cho thống nhất)
        var userId = HttpContext.Session.GetInt32("MaND");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ThanhToan", "GioHang") });
        }

        // Lấy giỏ hàng
        var gioHang = _context.GioHangs
            .Include(g => g.MaSachNavigation)
            .Where(g => g.MaND == userId)
            .ToList();

        if (!gioHang.Any())
        {
            TempData["Error"] = "Giỏ hàng rỗng!";
            return RedirectToAction("Index", "GioHang");
        }

        // Tạo đơn hàng
        var donHang = new DonHang
        {
            MaND = userId.Value,
            NgayDat = DateTime.Now,
            TongTien = gioHang.Sum(g => g.SoLuong * g.MaSachNavigation.GiaBan),
            TrangThai = "Chờ xác nhận",
            DiaChiGiao = DiaChi
        };

        _context.DonHangs.Add(donHang);
        _context.SaveChanges();

        // Thêm chi tiết đơn hàng
        foreach (var item in gioHang)
        {
            var ctdh = new CT_DonHang
            {
                MaDH = donHang.MaDH,
                MaSach = item.MaSach,
                SoLuong = item.SoLuong,
                DonGia = item.MaSachNavigation.GiaBan
            };
            _context.CT_DonHangs.Add(ctdh);
        }

        // Xóa giỏ hàng sau khi đặt
        _context.GioHangs.RemoveRange(gioHang);
        _context.SaveChanges();

        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction("ChiTiet", new { id = donHang.MaDH });
    }

    /// <summary>
    /// Xem chi tiết đơn hàng
    /// </summary>
    public IActionResult ChiTiet(int id)
    {
        var userId = HttpContext.Session.GetInt32("MaND");
        if (userId == null) return RedirectToAction("Login", "Account");

        var donHang = _context.DonHangs
            .Include(d => d.CT_DonHangs)
                .ThenInclude(ct => ct.MaSachNavigation)
            .Include(d => d.MaNDNavigation)
            .FirstOrDefault(d => d.MaDH == id && d.MaND == userId);

        if (donHang == null) return NotFound();

        return View(donHang);
    }

    /// <summary>
    /// Danh sách đơn hàng của user
    /// </summary>
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("MaND");
        if (userId == null) return RedirectToAction("Login", "Account");

        var donHangs = _context.DonHangs
            .Where(d => d.MaND == userId)
            .OrderByDescending(d => d.NgayDat)
            .ToList();

        return View(donHangs);
    }
}
