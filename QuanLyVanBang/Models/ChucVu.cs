using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyVanBang.Models
{
    [Table("ChucVu")]
    public class ChucVu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã chức vụ")]
        public int MaChucVu { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chức vụ")]
        [StringLength(255)]
        [Display(Name = "Tên chức vụ")]
        public string TenChucVu { get; set; }
        [StringLength(10)] 
        [Display(Name = "Kí hiệu")]

        public string? KiHieu { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        public virtual ICollection<NguoiDung>? NguoiDungs { get; set; }
    }
}
