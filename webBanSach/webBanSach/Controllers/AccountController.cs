using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Areas.User.Controllers
{
    public class AccountController : Controller
    {
        private readonly WebBanSachContext _context;

        public AccountController(WebBanSachContext context)
        {
            _context = context;
        }

        // GET: User/Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == model.Email
                                  && u.LoaiNguoiDung == "User"
                                  && u.TrangThai == "Hoạt động");

            if (user != null && BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau))
            {
                HttpContext.Session.SetInt32("MaND", user.MaND);
                HttpContext.Session.SetString("UserName", user.HoTen ?? "User");

                string avatarPath = string.IsNullOrEmpty(user.HinhAnh)
                    ? "/images/nguoidung/default.png"
                    : $"/images/nguoidung/{user.HinhAnh}";
                HttpContext.Session.SetString("UserAvatar", avatarPath);

                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View(model);
        }

        // GET: User/Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_context.NguoiDungs.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã tồn tại.";
                return View(model);
            }

            var user = new NguoiDung
            {
                HoTen = model.HoTen,
                Email = model.Email,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                SDT = model.SDT,
                DiaChi = model.DiaChi,
                LoaiNguoiDung = "User",
                TrangThai = "Hoạt động",
                NgayTao = DateTime.Now,
                HinhAnh = "default.png"
            };

            _context.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: User/Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
