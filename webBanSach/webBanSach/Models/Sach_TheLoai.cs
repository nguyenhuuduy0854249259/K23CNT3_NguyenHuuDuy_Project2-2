using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("Sach_TheLoai")]
public partial class Sach_TheLoai
{
    public int MaSach { get; set; }
    public int MaLoai { get; set; }

    [ForeignKey("MaSach")]
    [InverseProperty("Sach_TheLoais")]
    public virtual Sach MaSachNavigation { get; set; } = null!;

    [ForeignKey("MaLoai")]
    [InverseProperty("Sach_TheLoais")]
    public virtual TheLoai MaLoaiNavigation { get; set; } = null!;
}
