using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("Sach_TacGia")]
public partial class Sach_TacGia
{
    public int MaSach { get; set; }
    public int MaTG { get; set; }

    [ForeignKey("MaSach")]
    [InverseProperty("Sach_TacGias")]
    public virtual Sach MaSachNavigation { get; set; } = null!;

    [ForeignKey("MaTG")]
    [InverseProperty("Sach_TacGias")]
    public virtual TacGia MaTGNavigation { get; set; } = null!;
}