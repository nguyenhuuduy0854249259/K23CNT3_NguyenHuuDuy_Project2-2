using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("DonHang")]
public partial class DonHang
{
    [Key]
    public int MaDH { get; set; }

    public int MaND { get; set; }

    public int? MaKM { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDat { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTien { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [StringLength(255)]
    public string? DiaChiGiao { get; set; }

    [InverseProperty("MaDHNavigation")]
    public virtual ICollection<CT_DonHang> CT_DonHangs { get; set; } = new List<CT_DonHang>();

    [ForeignKey("MaKM")]
    [InverseProperty("DonHangs")]
    public virtual KhuyenMai? MaKMNavigation { get; set; }

    [ForeignKey("MaND")]
    [InverseProperty("DonHangs")]
    public virtual NguoiDung MaNDNavigation { get; set; } = null!;
}
