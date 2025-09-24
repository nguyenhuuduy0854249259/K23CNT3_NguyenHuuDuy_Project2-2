using System.Collections.Generic;

namespace webBanSach.ViewModels
{
    public class HomeViewModel
    {
        // Danh sách sách nổi bật
        public List<SachViewModel> SachNoiBat { get; set; } = new List<SachViewModel>();

        // Danh sách sách bán chạy
        public List<SachViewModel> SachBanChay { get; set; } = new List<SachViewModel>();
    }
}
