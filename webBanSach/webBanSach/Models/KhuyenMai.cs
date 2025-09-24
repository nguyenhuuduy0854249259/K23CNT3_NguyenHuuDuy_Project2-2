using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webBanSach.Models;

[Table("KhuyenMai")]
[Index("MaCode", IsUnique = true)]
public partial class KhuyenMai
{
    [Key]
    public int MaKM { get; set; }

    [StringLength(50)]
    public string MaCode { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? PhanTramGiam { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? GiamToiDa { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("MaKMNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
