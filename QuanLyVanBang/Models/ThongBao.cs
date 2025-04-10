using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    public class ThongBao
    {
        [Key]
        public int MaThongBao { get; set; }
        [Required]
        public string NguoiNhan { get; set; }
        [Required]
        [StringLength(500)]
        public string NoiDung { get; set; }
        [Required]
        public DateTime NgayTao { get; set; }
        [Required]
        public bool DaXem { get; set; }
        [Required]
        public int LoaiThongBao { get; set; } // 1: Thông báo văn bản đến, 2: Thông báo duyệt, 3: Thông báo từ chối
        public string? DuongDanVanBan { get; set; } // Đường dẫn đến trang xử lý văn bản
        [ForeignKey("NguoiNhan")]
        public virtual NguoiDung NguoiDungNhan { get; set; }
    }
} 