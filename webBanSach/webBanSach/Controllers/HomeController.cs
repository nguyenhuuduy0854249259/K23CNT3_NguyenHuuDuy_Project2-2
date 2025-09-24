using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebBanSachContext _context;

        public HomeController(ILogger<HomeController> logger, WebBanSachContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ? Trang ch?
        public async Task<IActionResult> Index()
        {
            // L?y danh sách sách n?i b?t
            var noiBat = await _context.Saches
                .Where(s => s.LuotXem >= 5)
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh
                })
                .ToListAsync();

            // L?y danh sách sách bán ch?y (top 5)
            var allBooks = await _context.Saches
                .Select(s => new SachViewModel
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaBan = s.GiaBan,
                    SoLuong = s.SoLuong,
                    MoTa = s.MoTa,
                    HinhAnh = s.HinhAnh
                })
                .ToListAsync();

            // ? Tính s? l??ng ?ã bán cho t?t c? sách
            var daBanDict = await _context.CT_DonHangs
                .GroupBy(ct => ct.MaSach)
                .Select(g => new { MaSach = g.Key, DaBan = g.Sum(x => x.SoLuong) })
                .ToDictionaryAsync(x => x.MaSach, x => x.DaBan);

            // L?c ra sách bán ch?y > 5
            var banChay = allBooks
                .Where(s => daBanDict.ContainsKey(s.MaSach) && daBanDict[s.MaSach] >=5)
                .OrderByDescending(s => daBanDict[s.MaSach])
                .Take(5)
                .ToList();

            // G?i dictionary xu?ng view qua ViewBag
            ViewBag.DaBan = daBanDict;

            var vm = new HomeViewModel
            {
                SachNoiBat = noiBat,
                SachBanChay = banChay
            };

            return View(vm);
        }

        // Trang gi?i thi?u
        public IActionResult About()
        {
            return View();
        }

        // Trang l?i
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
