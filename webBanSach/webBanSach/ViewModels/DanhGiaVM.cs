using System.ComponentModel.DataAnnotations;

namespace webBanSach.ViewModels
{
    public class DanhGiaVM
    {
        public int MaSach { get; set; }

        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5")]
        public int Diem { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập bình luận")]
        [StringLength(500)]
        public string BinhLuan { get; set; } = string.Empty;
    }
}
