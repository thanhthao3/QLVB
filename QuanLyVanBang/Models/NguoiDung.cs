using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [StringLength(255)]
        [Display(Name = "Mã người dùng")]
        public string MaNguoiDung { get; set; } 

        [Required]
        [StringLength(255)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Required]
        [StringLength(255)] 
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required]
        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; } // bit -> bool

        [Required]
        [StringLength(255)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Required]
        [StringLength(11)]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Required]      
        [Display(Name = "Mã chức vụ")]
        public int MaChucVu { get; set; } 

        [Required]
        [Display(Name = "Mã đơn vị")]
        public int MaDonVi { get; set; } 

        [Required]
        [StringLength(255)] 
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required]
        [Display(Name = "Vai trò")]
        public int Role { get; set; } 

        [ForeignKey("MaChucVu")]
        [Display(Name = "Chức vụ")]
        public virtual ChucVu? ChucVu { get; set; }

        [ForeignKey("MaDonVi")]
        [Display(Name = "Đơn vị")]
        public virtual DonVi? DonVi { get; set; }

        public virtual ICollection<VanBanDi>? VanBanDis { get; set; }
        public virtual ICollection<VanBanDen>? VanBanDens { get; set; }
        public virtual ICollection<LichSuXuLy>? LichSuXuLys { get; set; }
    }
}
