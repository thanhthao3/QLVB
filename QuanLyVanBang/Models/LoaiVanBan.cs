using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    [Table("LoaiVanBan")]
    public class LoaiVanBan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã loại văn bản")]
        public int MaLoaiVanBan { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên loại văn bản")]
        public string TenLoai { get; set; }

        [StringLength(4)]   
        [Display(Name = "Kí hiệu")]
        public string? KiHieu { get; set; } // Có thể null

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; } // nvarchar(MAX) có thể null
    }
}
