using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webBanSach.Models;

[Table("TheLoai")]
[Index("TenLoai", IsUnique = true)]
public partial class TheLoai
{
    [Key]
    public int MaLoai { get; set; }

    [StringLength(100)]
    public string TenLoai { get; set; } = null!;

    public virtual ICollection<Sach_TheLoai> Sach_TheLoais { get; set; } = new List<Sach_TheLoai>();
}
