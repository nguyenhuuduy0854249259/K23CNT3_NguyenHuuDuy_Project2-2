using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Controllers
{
    public class SachController : Controller
    {
        private readonly WebBanSachContext _context;

        public SachController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: Sach
        public async Task<IActionResult> Index()
        {
            var listSach = await _context.Saches
                .Include(s => s.MaNXBNavigation) // load thêm NXB
                .ToListAsync();
            return View(listSach);
        }

        public async Task<IActionResult> Details(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.MaNXBNavigation)
                .Include(s => s.Sach_TheLoais).ThenInclude(stl => stl.MaLoaiNavigation)
                .Include(s => s.Sach_TacGias).ThenInclude(stg => stg.MaTGNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);

            if (sach == null)
            {
                return NotFound();
            }

            // tăng lượt xem
            sach.LuotXem++;
            _context.Update(sach);
            await _context.SaveChangesAsync();

            // Lưu đường dẫn gốc để hiển thị nút quay lại
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl)
                ? Url.Action("Index", "Sach")
                : returnUrl;

            return View(sach);
        }


        // ✅ Tìm sách theo thể loại (N-N)
        public IActionResult SearchByCategory(string theloai)
        {
            if (string.IsNullOrEmpty(theloai))
            {
                return RedirectToAction("Index", "Home");
            }

            var sachTheoLoai = _context.Saches
                .Where(s => s.Sach_TheLoais.Any(st => st.MaLoaiNavigation.TenLoai == theloai))
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh
                })
                .ToList();

            ViewBag.TheLoai = theloai;
            return View(sachTheoLoai);
        }

        // ✅ Tìm sách theo tác giả (N-N)
        public IActionResult SearchByAuthor(string tacgia)
        {
            if (string.IsNullOrEmpty(tacgia))
            {
                return RedirectToAction("Index", "Home");
            }

            var sachTheoTacGia = _context.Saches
                .Where(s => s.Sach_TacGias.Any(st => st.MaTGNavigation.TenTG == tacgia))
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh
                })
                .ToList();

            ViewBag.TacGia = tacgia;
            return View(sachTheoTacGia);
        }

        // ✅ Tìm sách theo NXB
        public IActionResult SearchByPublisher(string nxb)
        {
            if (string.IsNullOrEmpty(nxb))
            {
                return RedirectToAction("Index", "Home");
            }

            var sachTheoNXB = _context.Saches
                .Where(s => s.MaNXBNavigation.TenNXB == nxb)
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh
                })
                .ToList();

            ViewBag.NXB = nxb;
            return View(sachTheoNXB);
        }

        private bool SachExists(int id)
        {
            return _context.Saches.Any(e => e.MaSach == id);
        }
    }
}
