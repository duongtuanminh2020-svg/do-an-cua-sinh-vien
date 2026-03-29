using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace webquanli.Models
{
    public class TienDo
    {
        public int Id { get; set; }

        public int DangKyId { get; set; }

        public string NoiDung { get; set; }

        public int PhanTram { get; set; }

        public DateTime NgayCapNhat { get; set; }

        [ForeignKey("DangKyId")]
        public DangKyDeTai DangKyDeTai { get; set; }
    }
}