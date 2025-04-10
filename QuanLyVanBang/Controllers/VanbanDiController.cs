using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;
using System.IO;

namespace QuanLyVanBang.Controllers
{
    public class VanbanDiController : BaseController
    {
        private readonly QuanLyVanBangDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VanbanDiController(QuanLyVanBangDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: VanbanDi
        public async Task<IActionResult> Index()
        {
            if(!IsLogin)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }else {
                var currentUserId = CurrentUserID;

                // Lấy danh sách văn bản đi mà người dùng là người gửi
                var vanBanDis = await _context.VanBanDis
                    .Where(v => v.MaNguoiGui == currentUserId)
                    .Include(v => v.NguoiGui)
                    .Include(v => v.LoaiVanBan)
                    .Include(v => v.DonVi)
                    .OrderByDescending(v => v.NgayGui)
                    .ToListAsync();

                return View(vanBanDis);
            }     
        }

        // GET: VanbanDi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vanbanDi = await _context.VanBanDis
                .FirstOrDefaultAsync(m => m.MaVanBanDi == id);
            if (vanbanDi == null)
            {
                return NotFound();
            }
            return View(vanbanDi);
        }

        // GET: VanbanDi/Create
        public IActionResult Create()
        {
            ViewBag.LoaiVanBan = _context.LoaiVanBans.ToList();
            ViewBag.DonViBanHanh = _context.DonVis.ToList();
            ViewBag.NguoiDung = _context.NguoiDungs.Include(nd => nd.ChucVu).ToList();
            ViewBag.DonVi = _context.DonVis.ToList();
            return View();
        }

        // POST: VanbanDi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVanBanDi,SoHieuVanBan,TrichYeu,MaLoaiVanBan,DoKhan,DonViBanHanh,Nam,NgayGui,GhiChu,TrangThai,TepDinhKem")] VanBanDi vanbanDi,
            IFormFile tepDinhKem, List<LichSuXuLy> lichSuXuLys)
        {
            try
            {
                // Gán MaNguoiGui và TrangThai trước khi kiểm tra ModelState
                vanbanDi.MaNguoiGui = CurrentUserID;
                vanbanDi.TrangThai = 1; // Chờ duyệt
                vanbanDi.NgayGui = DateTime.Now;

                // Xóa lỗi ModelState cho MaNguoiGui nếu có
                ModelState.Remove("MaNguoiGui");

                if (!ModelState.IsValid)
                {
                    ViewBag.LoaiVanBan = _context.LoaiVanBans.ToList();
                    ViewBag.DonViBanHanh = _context.DonVis.ToList();
                    ViewBag.NguoiDung = _context.NguoiDungs.Include(nd => nd.ChucVu).ToList();
                    ViewBag.DonVi = _context.DonVis.ToList();
                    return View(vanbanDi);
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Xử lý upload file
                        if (tepDinhKem != null && tepDinhKem.Length > 0)
                        {
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            string uniqueFileName = GetUniqueFileName(tepDinhKem.FileName, uploadsFolder);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await tepDinhKem.CopyToAsync(fileStream);
                            }

                            vanbanDi.TepDinhKem = uniqueFileName;
                        }

                        // Lưu văn bản đi
                        _context.Add(vanbanDi);
                        await _context.SaveChangesAsync();

                        // Lưu lịch sử xử lý và tạo văn bản đến cho người xử lý bước 1
                        if (lichSuXuLys != null && lichSuXuLys.Any())
                        {
                            var firstStep = lichSuXuLys.OrderBy(l => l.Buoc).First();
                            
                            // Tạo văn bản đến cho người xử lý bước 1
                            var vanBanDen = new VanBanDen
                            {
                                MaVanBanDi = vanbanDi.MaVanBanDi,
                                NguoiNhan = firstStep.MaNguoiDuyet,
                                NgayNhan = DateTime.Now,
                                TrangThai = 2 // Chưa duyệt - chỉ người bước 1 mới thấy văn bản
                            };
                            _context.VanBanDens.Add(vanBanDen);
                            await _context.SaveChangesAsync();

                            // Lưu tất cả lịch sử xử lý
                            foreach (var lichSu in lichSuXuLys)
                            {
                                lichSu.MaVanBanDi = vanbanDi.MaVanBanDi;
                                lichSu.NgayDuyet = DateTime.Now;
                                
                                // Nếu là người bước 1
                                if (lichSu.Buoc == firstStep.Buoc)
                                {
                                    lichSu.MaVanBanDen = vanBanDen.MaVanBanDen;
                                    lichSu.TrangThai = 2; // Chưa duyệt
                                }
                                else
                                {
                                    // Tạo văn bản đến cho các bước sau với trạng thái đợi duyệt
                                    var vanBanDenNext = new VanBanDen
                                    {
                                        MaVanBanDi = vanbanDi.MaVanBanDi,
                                        NguoiNhan = lichSu.MaNguoiDuyet,
                                        NgayNhan = DateTime.Now,
                                        TrangThai = 1 // Đợi duyệt
                                    };
                                    _context.VanBanDens.Add(vanBanDenNext);
                                    await _context.SaveChangesAsync();

                                    lichSu.MaVanBanDen = vanBanDenNext.MaVanBanDen;
                                    lichSu.TrangThai = 1; // Đợi duyệt
                                }

                                _context.LichSuXuLys.Add(lichSu);
                            }
                            await _context.SaveChangesAsync();

                            // Tạo thông báo cho người xử lý bước 1
                            var thongBao = new ThongBao
                            {
                                NguoiNhan = firstStep.MaNguoiDuyet,
                                NoiDung = $"Bạn có văn bản mới cần xử lý: {vanbanDi.SoHieuVanBan}",
                                NgayTao = DateTime.Now,
                                DaXem = false,
                                LoaiThongBao = 1, // Thông báo văn bản đến
                                DuongDanVanBan = $"/VanbanDen/Process/{vanBanDen.MaVanBanDen}"
                            };
                            _context.ThongBaos.Add(thongBao);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Lỗi khi lưu dữ liệu: " + ex.Message + "\n" + ex.InnerException?.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                
                ViewBag.LoaiVanBan = _context.LoaiVanBans.ToList();
                ViewBag.DonViBanHanh = _context.DonVis.ToList();
                ViewBag.NguoiDung = _context.NguoiDungs.Include(nd => nd.ChucVu).ToList();
                ViewBag.DonVi = _context.DonVis.ToList();
                return View(vanbanDi);
            }
        }

        private string GetUniqueFileName(string fileName, string uploadsFolder)
        {
            string uniqueFileName = fileName;
            string extension = Path.GetExtension(fileName);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            int counter = 1;

            while (System.IO.File.Exists(Path.Combine(uploadsFolder, uniqueFileName)))
            {
                uniqueFileName = $"{nameWithoutExtension}_{counter}{extension}";
                counter++;
            }

            return uniqueFileName;
        }

        // GET: VanbanDi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vanbanDi = await _context.VanBanDis.FindAsync(id);
            if (vanbanDi == null)
            {
                return NotFound();
            }
            return View(vanbanDi);
        }

        // POST: VanbanDi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaVanBanDi,SoHieuVanBan,TrichYeu,MaLoaiVanBan,DoKhan,DonViBanHanh,TepDinhKem,Nam,NgayGui,GhiChu,MaNguoiGui,TrangThai")] VanBanDi vanbanDi)
        {
            if (id != vanbanDi.MaVanBanDi)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vanbanDi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VanbanDiExists(vanbanDi.MaVanBanDi))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vanbanDi);
        }

        // GET: VanbanDi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vanbanDi = await _context.VanBanDis
                .FirstOrDefaultAsync(m => m.MaVanBanDi == id);
            if (vanbanDi == null)
            {
                return NotFound();
            }
            return View(vanbanDi);
        }

        // POST: VanbanDi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vanbanDi = await _context.VanBanDis.FindAsync(id);
            _context.VanBanDis.Remove(vanbanDi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VanbanDiExists(int id)
        {
            return _context.VanBanDis.Any(e => e.MaVanBanDi == id);
        }
    }
} 