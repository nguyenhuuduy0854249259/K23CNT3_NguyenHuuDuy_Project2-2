using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("LienHe")]
public partial class LienHe
{
    [Key]
    public int MaLH { get; set; }

    public int? MaND { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(200)]
    public string? TieuDe { get; set; }

    public string NoiDung { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaND")]
    [InverseProperty("LienHes")]
    public virtual NguoiDung? MaNDNavigation { get; set; }
}
