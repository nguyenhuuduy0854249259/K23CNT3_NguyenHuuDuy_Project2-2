using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.Email == model.Email
                                  && u.LoaiNguoiDung == "User"
                                  && u.TrangThai == "Hoạt động");

            if (user != null && BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau))
            {
                // ✅ Lưu session user
                HttpContext.Session.SetInt32("MaND", user.MaND);
                HttpContext.Session.SetString("UserName", user.HoTen ?? "User");

                string avatarPath = string.IsNullOrEmpty(user.HinhAnh)
                    ? "/images/nguoidung/default.png"
                    : $"/images/nguoidung/{user.HinhAnh}";
                HttpContext.Session.SetString("UserAvatar", avatarPath);

                // ✅ Nếu có returnUrl thì chuyển về trang đó
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Nếu không có returnUrl thì quay về Home
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: User/Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (_context.NguoiDungs.Any(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã tồn tại.";
                ViewBag.ReturnUrl = returnUrl;
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

            // ✅ Sau khi đăng ký, điều hướng sang Login, giữ returnUrl
            return RedirectToAction("Login", new { returnUrl });
        }
        // GET: User/Account/Logout
        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Clear();

            // Sau khi đăng xuất → quay về trang chủ ngoài Area
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
