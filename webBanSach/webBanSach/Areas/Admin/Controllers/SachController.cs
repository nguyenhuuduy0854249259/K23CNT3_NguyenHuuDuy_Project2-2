using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webBanSach.Models;
using webBanSach.ViewModels;

namespace webBanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SachController : Controller
    {
        private readonly WebBanSachContext _context;
        private readonly IWebHostEnvironment _env;

        public SachController(WebBanSachContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        // GET: Admin/Sach
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.Saches
                .Include(s => s.MaNXBNavigation)
                .Include(s => s.Sach_TacGias).ThenInclude(st => st.MaTGNavigation)
                .Include(s => s.Sach_TheLoais).ThenInclude(sl => sl.MaLoaiNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.TenSach.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        // GET: Admin/Sach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.MaNXBNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);

            if (sach == null) return NotFound();

            // ✅ Tăng lượt xem
            sach.LuotXem++;
            _context.Update(sach);
            await _context.SaveChangesAsync();

            // ✅ Tính số lượng đã bán
            ViewBag.DaBan = await _context.CT_DonHangs
                .Where(ct => ct.MaSach == sach.MaSach)
                .SumAsync(ct => (int?)ct.SoLuong) ?? 0;

            return View(sach);
        }

        // GET: Admin/Sach/Create
        public IActionResult Create()
        {
            var vm = new SachViewModel
            {
                TacGias = _context.TacGias.Select(t => new SelectListItem { Value = t.MaTG.ToString(), Text = t.TenTG }),
                TheLoais = _context.TheLoais.Select(l => new SelectListItem { Value = l.MaLoai.ToString(), Text = l.TenLoai }),
                NhaXuatBans = _context.NhaXuatBans.Select(n => new SelectListItem { Value = n.MaNXB.ToString(), Text = n.TenNXB })
            };
            return View(vm);
        }

        // POST: Admin/Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SachViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var sach = new Sach
                {
                    TenSach = vm.TenSach,
                    MaNXB = vm.MaNXB,
                    NamXB = vm.NamXB,
                    GiaBan = vm.GiaBan,
                    SoLuong = vm.SoLuong,
                    MoTa = vm.MoTa,
                    LuotXem = 0
                };

                // Upload ảnh
                if (vm.HinhAnhFile != null && vm.HinhAnhFile.Length > 0)
                {
                    string folder = Path.Combine(_env.WebRootPath, "images", "sach");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Path.GetFileName(vm.HinhAnhFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await vm.HinhAnhFile.CopyToAsync(stream);
                    }
                    sach.HinhAnh = fileName;
                }

                _context.Saches.Add(sach);
                await _context.SaveChangesAsync();

                // Liên kết tác giả
                foreach (var tgId in vm.SelectedTacGia)
                {
                    _context.Sach_TacGias.Add(new Sach_TacGia { MaSach = sach.MaSach, MaTG = tgId });
                }

                // Liên kết thể loại
                foreach (var loaiId in vm.SelectedTheLoai)
                {
                    _context.Sach_TheLoais.Add(new Sach_TheLoai { MaSach = sach.MaSach, MaLoai = loaiId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.TacGias = _context.TacGias.Select(t => new SelectListItem { Value = t.MaTG.ToString(), Text = t.TenTG });
            vm.TheLoais = _context.TheLoais.Select(l => new SelectListItem { Value = l.MaLoai.ToString(), Text = l.TenLoai });
            vm.NhaXuatBans = _context.NhaXuatBans.Select(n => new SelectListItem { Value = n.MaNXB.ToString(), Text = n.TenNXB });
            return View(vm);
        }

        // GET: Admin/Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sach = await _context.Saches
                .Include(s => s.Sach_TacGias)
                .Include(s => s.Sach_TheLoais)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach == null) return NotFound();

            var vm = new SachViewModel
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                MaNXB = sach.MaNXB,
                NamXB = sach.NamXB,
                GiaBan = sach.GiaBan,
                SoLuong = sach.SoLuong,
                MoTa = sach.MoTa,
                HinhAnh = sach.HinhAnh,
                SelectedTacGia = sach.Sach_TacGias.Select(st => st.MaTG).ToList(),
                SelectedTheLoai = sach.Sach_TheLoais.Select(sl => sl.MaLoai).ToList(),
                TacGias = _context.TacGias.Select(t => new SelectListItem { Value = t.MaTG.ToString(), Text = t.TenTG }),
                TheLoais = _context.TheLoais.Select(l => new SelectListItem { Value = l.MaLoai.ToString(), Text = l.TenLoai }),
                NhaXuatBans = _context.NhaXuatBans.Select(n => new SelectListItem { Value = n.MaNXB.ToString(), Text = n.TenNXB })
            };

            return View(vm);
        }

        // POST: Admin/Sach/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SachViewModel vm)
        {
            if (id != vm.MaSach) return NotFound();

            if (ModelState.IsValid)
            {
                var sach = await _context.Saches
                    .Include(s => s.Sach_TacGias)
                    .Include(s => s.Sach_TheLoais)
                    .FirstOrDefaultAsync(s => s.MaSach == id);

                if (sach == null) return NotFound();

                // Cập nhật dữ liệu
                sach.TenSach = vm.TenSach;
                sach.MaNXB = vm.MaNXB;
                sach.NamXB = vm.NamXB;
                sach.GiaBan = vm.GiaBan;
                sach.SoLuong = vm.SoLuong;
                sach.MoTa = vm.MoTa;

                // Upload ảnh mới (xóa ảnh cũ)
                if (vm.HinhAnhFile != null && vm.HinhAnhFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(sach.HinhAnh))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, "images", "sach", sach.HinhAnh);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    string folder = Path.Combine(_env.WebRootPath, "images", "sach");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Path.GetFileName(vm.HinhAnhFile.FileName);
                    string path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await vm.HinhAnhFile.CopyToAsync(stream);
                    }
                    sach.HinhAnh = fileName;
                }

                // Cập nhật tác giả
                _context.Sach_TacGias.RemoveRange(sach.Sach_TacGias);
                foreach (var tgId in vm.SelectedTacGia)
                {
                    _context.Sach_TacGias.Add(new Sach_TacGia { MaSach = sach.MaSach, MaTG = tgId });
                }

                // Cập nhật thể loại
                _context.Sach_TheLoais.RemoveRange(sach.Sach_TheLoais);
                foreach (var loaiId in vm.SelectedTheLoai)
                {
                    _context.Sach_TheLoais.Add(new Sach_TheLoai { MaSach = sach.MaSach, MaLoai = loaiId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.TacGias = _context.TacGias.Select(t => new SelectListItem { Value = t.MaTG.ToString(), Text = t.TenTG });
            vm.TheLoais = _context.TheLoais.Select(l => new SelectListItem { Value = l.MaLoai.ToString(), Text = l.TenLoai });
            vm.NhaXuatBans = _context.NhaXuatBans.Select(n => new SelectListItem { Value = n.MaNXB.ToString(), Text = n.TenNXB });
            return View(vm);
        }

        // POST: Admin/Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches
                .Include(s => s.Sach_TacGias)
                .Include(s => s.Sach_TheLoais)
                .FirstOrDefaultAsync(s => s.MaSach == id);

            if (sach != null)
            {
                // Xóa ảnh
                if (!string.IsNullOrEmpty(sach.HinhAnh))
                {
                    var filePath = Path.Combine(_env.WebRootPath, "images", "sach", sach.HinhAnh);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                // Xóa liên kết
                _context.Sach_TacGias.RemoveRange(sach.Sach_TacGias);
                _context.Sach_TheLoais.RemoveRange(sach.Sach_TheLoais);

                _context.Saches.Remove(sach);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
