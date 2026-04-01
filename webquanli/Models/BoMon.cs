using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webquanli.Models
{
    public class BoMon
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên bộ môn không được để trống")]
        public string TenBoMon { get; set; }

        public ICollection<GiangVien>? GiangViens { get; set; }
    }
}