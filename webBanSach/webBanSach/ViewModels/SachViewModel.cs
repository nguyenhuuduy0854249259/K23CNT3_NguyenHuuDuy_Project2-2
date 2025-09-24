using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webBanSach.ViewModels   // ✅ Namespace phải đúng
{
    public class SachViewModel
    {
        public int MaSach { get; set; }

        [Required]
        [StringLength(200)]
        public string TenSach { get; set; } = null!;

        public int MaNXB { get; set; }
        public int? NamXB { get; set; }
        public decimal GiaBan { get; set; }
        public int? SoLuong { get; set; }
        public string? MoTa { get; set; }
        public string? HinhAnh { get; set; }

        // Upload file
        public IFormFile? HinhAnhFile { get; set; }

        // Dropdown list NXB
        public IEnumerable<SelectListItem>? NhaXuatBans { get; set; }

        // Multiple select Tác giả
        public List<int>? SelectedTacGia { get; set; }
        public IEnumerable<SelectListItem>? TacGias { get; set; }

        // Multiple select Thể loại
        public List<int>? SelectedTheLoai { get; set; }
        public IEnumerable<SelectListItem>? TheLoais { get; set; }
    }
}
