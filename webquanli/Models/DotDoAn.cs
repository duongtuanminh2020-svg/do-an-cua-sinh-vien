using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webquanli.Models
{
    public class DotDoAn
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đợt không được để trống")]
        public string TenDot { get; set; } // Ví dụ: "Báo cáo tiến độ đợt 1", "Nộp báo cáo cuối kỳ"

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        public bool IsActive { get; set; } = true;

        // Liên kết với các báo cáo thuộc đợt này
        public ICollection<BaoCao>? BaoCaos { get; set; }
    }
}