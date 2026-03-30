using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webquanli.Models
{
    public class BaoCao
    {
        public int Id { get; set; }

        // Cột cũ trong SQL bắt buộc phải có dữ liệu
        public int DangKyId { get; set; }

        // Cột khóa ngoại thực sự của SQL
        public int DangKyDeTaiId { get; set; }

        [Required]
        public string TenFile { get; set; }

        [Required]
        public string DuongDan { get; set; }

        public DateTime NgayNop { get; set; } = DateTime.Now;

        [ForeignKey("DangKyDeTaiId")]
        public virtual DangKyDeTai DangKyDeTai { get; set; }
    }
}