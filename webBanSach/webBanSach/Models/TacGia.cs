using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webBanSach.Models;

[Table("TacGia")]
public partial class TacGia
{
    [Key]
    public int MaTG { get; set; }

    [StringLength(100)]
    public string TenTG { get; set; } = null!;

    public virtual ICollection<Sach_TacGia> Sach_TacGias { get; set; } = new List<Sach_TacGia>();
}
