using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webBanSach.Models;

[Table("NguoiDung")]
[Index("Email", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    public int MaND { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;


    [StringLength(15)]
    public string? SDT { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [StringLength(255)]
    public string? HinhAnh { get; set; }

    [StringLength(20)]
    public string? LoaiNguoiDung { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaNDNavigation")]
    public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();


    [InverseProperty("MaNDNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    [InverseProperty("MaNDNavigation")]
    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    [InverseProperty("MaNDNavigation")]
    public virtual ICollection<LienHe> LienHes { get; set; } = new List<LienHe>();
}
