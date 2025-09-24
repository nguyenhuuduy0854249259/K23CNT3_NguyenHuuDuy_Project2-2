using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("DanhGia")]
public partial class DanhGia
{
    [Key]
    public int MaDG { get; set; }

    public int MaSach { get; set; }

    public int MaND { get; set; }

    public int? Diem { get; set; }

    public string? BinhLuan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDG { get; set; }

    [ForeignKey("MaND")]
    [InverseProperty("DanhGias")]
    public virtual NguoiDung MaNDNavigation { get; set; } = null!;

    [ForeignKey("MaSach")]
    [InverseProperty("DanhGias")]
    public virtual Sach MaSachNavigation { get; set; } = null!;
}
