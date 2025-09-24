using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("GioHang")]
[PrimaryKey(nameof(MaND), nameof(MaSach))]
public partial class GioHang
{
    [Key]
    public int MaND { get; set; }

    [Key]
    public int MaSach { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [ForeignKey("MaND")]
    [InverseProperty("GioHangs")]
    public virtual NguoiDung MaNDNavigation { get; set; } = null!;

    [ForeignKey("MaSach")]
    [InverseProperty("GioHangs")]
    public virtual Sach MaSachNavigation { get; set; } = null!;
}
