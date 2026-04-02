using System.ComponentModel.DataAnnotations.Schema;

namespace webquanli.Models
{
    public class GiangVien
    {
        public int Id { get; set; }

        public string MaGV { get; set; }

        public string TenGV { get; set; }

        public string Email { get; set; }

        public string Khoa { get; set; }

        // DÒNG DUY NHẤT CẦN THÊM VÀO ĐỂ SỬA LỖI VÀ CHẠY PHÂN QUYỀN:
        public string LopQuanLy { get; set; }

        public string? Avatar { get; set; }
        public string? SDT { get; set; }

        public int? BoMonId { get; set; }
        [ForeignKey("BoMonId")]
        public virtual BoMon? BoMon { get; set; }
    }
}