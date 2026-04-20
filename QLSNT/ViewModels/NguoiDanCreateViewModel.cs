using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLSNT.Models;

namespace QLSNT.ViewModels
{
    public class NguoiDanCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng Nhập họ và tên")]
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh thường trú")]
        public int MaTinhMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn xã thường trú")]
        public int MaXaMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ thường trú")]
        [StringLength(255)]
        public string DiaChiThuongTru { get; set; }

        // Để biết người dân nào đang nhập địa chỉ
        public string MaCCCD { get; set; }
    }
}
