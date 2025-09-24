using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("NhaXuatBan")]
public partial class NhaXuatBan
{
    [Key]
    public int MaNXB { get; set; }

    [StringLength(200)]
    public string TenNXB { get; set; } = null!;

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [StringLength(20)]
    public string? SDT { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [InverseProperty("MaNXBNavigation")]
    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
