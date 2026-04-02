using System;
using System.ComponentModel.DataAnnotations;

namespace webquanli.Models
{
    public class ThongBao
    {
        [Key]
        public int Id { get; set; }

        // Thêm dấu ? vào sau chữ string
        [Required]
        public string? UsernameNhan { get; set; }

        [Required]
        public string? TieuDe { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool DaDoc { get; set; } = false;
    }
}