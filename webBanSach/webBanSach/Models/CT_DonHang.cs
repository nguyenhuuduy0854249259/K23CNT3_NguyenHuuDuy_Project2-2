using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webBanSach.Models;

[Table("CT_DonHang")]
[PrimaryKey(nameof(MaDH), nameof(MaSach))]
public partial class CT_DonHang
{
    [Key]
    public int MaDH { get; set; }

    [Key]
    public int MaSach { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(29, 2)")]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaDH")]
    [InverseProperty("CT_DonHangs")]
    public virtual DonHang MaDHNavigation { get; set; } = null!;

    [ForeignKey("MaSach")]
    [InverseProperty("CT_DonHangs")]
    public virtual Sach MaSachNavigation { get; set; } = null!;
}
