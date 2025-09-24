using Microsoft.AspNetCore.Mvc;
using webBanSach.Models; // namespace của DbContext
using System.Linq;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly WebBanSachContext _context;

        public HomeController(WebBanSachContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Thống kê 4 box
            ViewBag.TotalBooks = _context.Saches.Count();
            ViewBag.TotalUsers = _context.NguoiDungs.Count();
            ViewBag.TotalOrders = _context.DonHangs.Count();
            ViewBag.TotalRevenue = _context.DonHangs.Sum(d => d.TongTien ?? 0);

            // Lấy doanh thu theo tháng (6 tháng gần nhất)
            var doanhThuTheoThang = _context.DonHangs
                .Where(d => d.NgayDat.HasValue)
                .GroupBy(d => new { d.NgayDat.Value.Year, d.NgayDat.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TongDoanhThu = g.Sum(x => x.TongTien ?? 0)
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Take(6) // 6 tháng gần nhất
                .ToList()
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            // Chuyển sang JSON để gửi xuống view
            ViewBag.DoanhThuTheoThang = Newtonsoft.Json.JsonConvert.SerializeObject(doanhThuTheoThang);

            return View();
        }
    }
}
