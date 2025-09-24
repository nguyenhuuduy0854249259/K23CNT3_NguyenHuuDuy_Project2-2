using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("Sach")]
public partial class Sach
{
    [Key]
    public int MaSach { get; set; }

    [StringLength(200)]
    public string TenSach { get; set; } = null!;

    public int MaNXB { get; set; }

    public int? NamXB { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaBan { get; set; }

    public int? SoLuong { get; set; }

    public string? MoTa { get; set; }

    [StringLength(255)]
    public string? HinhAnh { get; set; }
    public int LuotXem { get; set; } = 0;


    [InverseProperty("MaSachNavigation")]
    public virtual ICollection<CT_DonHang> CT_DonHangs { get; set; } = new List<CT_DonHang>();

    [InverseProperty("MaSachNavigation")]
    public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();


    [InverseProperty("MaSachNavigation")]
    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    [ForeignKey("MaNXB")]
    [InverseProperty("Saches")]
    public virtual NhaXuatBan MaNXBNavigation { get; set; } = null!;

    public virtual ICollection<Sach_TheLoai> Sach_TheLoais { get; set; } = new List<Sach_TheLoai>();

    public virtual ICollection<Sach_TacGia> Sach_TacGias { get; set; } = new List<Sach_TacGia>();
}
