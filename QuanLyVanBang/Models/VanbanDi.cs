using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    [Table("VanBanDi")]
    public class VanBanDi
    {
        [Key]   
        [Display(Name = "Mã văn bản đi")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaVanBanDi { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Số hiệu văn bản")]
        public string SoHieuVanBan { get; set; }

        [Required]
        [Display(Name = "Trích yếu")]
        public string TrichYeu { get; set; }

        [Required]
        [Display(Name = "Mã loại văn bản")]
        public int MaLoaiVanBan { get; set; }

        [Required]
        [Display(Name = "Độ khẩn")]
        public int DoKhan { get; set; }

        [Required]
        [Display(Name = "Đơn vị ban hành")]
        public int DonViBanHanh { get; set; }

        [StringLength(255)]
        [Display(Name = "Tên tập đính kèm")]
        public string? TepDinhKem { get; set; }

        [Required]
        [Display(Name = "Năm")]
        public int Nam { get; set; }

        [Required]
        [Display(Name = "Ngày gửi")]
        public DateTime NgayGui { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(255)]     
        [Display(Name = "Mã người gửi")]
        public string MaNguoiGui { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        // Navigation Properties
        [ForeignKey("MaNguoiGui")]
        public virtual NguoiDung? NguoiGui { get; set; }

        [ForeignKey("MaLoaiVanBan")]
        public virtual LoaiVanBan? LoaiVanBan { get; set; }

        [ForeignKey("DonViBanHanh")]
        public virtual DonVi? DonVi { get; set; }

        public virtual ICollection<LichSuXuLy>? LichSuXuLys { get; set; }
        public virtual ICollection<VanBanDen>? VanBanDens { get; set; }


    }
}
