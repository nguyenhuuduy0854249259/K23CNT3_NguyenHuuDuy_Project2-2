using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using Microsoft.AspNetCore.Http; // cần cho IHttpContextAccessor

namespace webBanSach
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔹 Lấy connection string từ appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("WebBanSachConnection");

            // 🔹 Đăng ký DbContext
            builder.Services.AddDbContext<WebBanSachContext>(options =>
                options.UseSqlServer(connectionString));

            // 🔹 Đăng ký HttpContextAccessor để dùng trong View (Layout)
            builder.Services.AddHttpContextAccessor();

            // 🔹 Bật Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian sống của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 🔹 Thêm MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 🔹 Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 🔹 Phải đặt trước Authorization
            app.UseSession();

            app.UseAuthorization();

            // 🔹 Cấu hình Area (Admin/User) nếu có
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // 🔹 Route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
