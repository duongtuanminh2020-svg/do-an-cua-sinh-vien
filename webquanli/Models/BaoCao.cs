using System;
using System.ComponentModel.DataAnnotations;

namespace webquanli.Models
{
    public class BaoCao
    {
        public int Id { get; set; }

        public int DangKyId { get; set; }

        [Required]
        public string TenFile { get; set; }

        [Required]
        public string DuongDan { get; set; }

        public DateTime NgayNop { get; set; } = DateTime.Now;

        public virtual DangKyDeTai DangKyDeTai { get; set; }
    }
}