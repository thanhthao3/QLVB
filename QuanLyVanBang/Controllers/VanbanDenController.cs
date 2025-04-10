using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class VanbanDenController : BaseController
    {
        private readonly QuanLyVanBangDBContext _context;

        public VanbanDenController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: VanbanDen
        public async Task<IActionResult> Index()
        {
            if(!IsLogin)
            {
                return RedirectToAction("Login", "TaiKhoan");

            }
            else {
                var currentUserId = CurrentUserID;

                 // Lấy danh sách văn bản đến mà người dùng là người nhận và không ở trạng thái đợi duyệt
                var vanBanDens = await _context.VanBanDens
                    .Where(v => v.NguoiNhan == currentUserId && v.TrangThai != 1) // Không hiển thị văn bản đợi duyệt
                    .Include(v => v.VanBanDi)
                        .ThenInclude(vbd => vbd.NguoiGui)
                    .Include(v => v.VanBanDi)
                        .ThenInclude(vbd => vbd.LoaiVanBan)
                    .Include(v => v.NguoiDung)
                    .OrderByDescending(v => v.VanBanDi.NgayGui)
                    .ToListAsync();

                return View(vanBanDens);
            }
           
        }

        // GET: VanbanDen/Process/5
        public async Task<IActionResult> Process(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vanBanDen = await _context.VanBanDens
                .Include(v => v.VanBanDi)
                    .ThenInclude(vbd => vbd.NguoiGui)
                .Include(v => v.VanBanDi)
                    .ThenInclude(vbd => vbd.LoaiVanBan)
                .Include(v => v.NguoiDung)
                .FirstOrDefaultAsync(v => v.MaVanBanDen == id);

            if (vanBanDen == null)
            {
                return NotFound();
            }

            // Lấy thông tin lịch sử xử lý của người dùng hiện tại
            var lichSuXuLy = await _context.LichSuXuLys
                .FirstOrDefaultAsync(l => l.MaVanBanDen == id && l.MaNguoiDuyet == CurrentUserID);

            if (lichSuXuLy == null)
            {
                return NotFound();
            }

            // Lấy thông tin người xử lý bước tiếp theo
            var nextStep = await _context.LichSuXuLys
                .Include(l => l.NguoiDuyet)
                .FirstOrDefaultAsync(l => l.MaVanBanDi == vanBanDen.MaVanBanDi && l.Buoc == lichSuXuLy.Buoc + 1);

            if (nextStep != null)
            {
                ViewBag.IsLastStep = false;
                ViewBag.NextPerson = nextStep.NguoiDuyet;
            }
            else
            {
                ViewBag.IsLastStep = true;
                ViewBag.NguoiDung = await _context.NguoiDungs
                    .Include(n => n.DonVi)
                    .Include(n => n.ChucVu)
                    .Where(n => n.MaNguoiDung != CurrentUserID)
                    .ToListAsync();
            }

            ViewBag.LichSuXuLy = lichSuXuLy;
            return View(vanBanDen);
        }

        // Thêm phương thức kiểm tra trước khi xử lý
        public async Task<IActionResult> ConfirmProcess(int id)
        {
            var vanBanDen = await _context.VanBanDens
                .Include(v => v.VanBanDi)
                .Include(v => v.VanBanDi.NguoiGui) // Include trực tiếp NguoiGui
                .FirstOrDefaultAsync(v => v.MaVanBanDen == id);

            if (vanBanDen == null)
            {
                return NotFound();
            }

            // Lấy thông tin người gửi từ bảng NguoiDung
            var nguoiGui = await _context.NguoiDungs
                .FirstOrDefaultAsync(n => n.MaNguoiDung == vanBanDen.VanBanDi.MaNguoiGui);

            var lichSuXuLy = await _context.LichSuXuLys
                .FirstOrDefaultAsync(l => l.MaVanBanDen == id && l.MaNguoiDuyet == CurrentUserID);

            if (lichSuXuLy == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu đã duyệt rồi thì không cho phép sửa
            if (lichSuXuLy.TrangThai == 3 || lichSuXuLy.TrangThai == 4)
            {
                TempData["Error"] = "Văn bản này đã được xử lý, không thể thay đổi trạng thái!";
                return RedirectToAction(nameof(Index));
            }

            var confirmModel = new ProcessConfirmViewModel
            {
                MaVanBanDen = id,
                SoHieuVanBan = vanBanDen.VanBanDi.SoHieuVanBan,
                TrichYeu = vanBanDen.VanBanDi.TrichYeu,
                NguoiGui = nguoiGui?.HoTen ?? "Không xác định",
                NgayGui = vanBanDen.VanBanDi.NgayGui
            };

            return View(confirmModel);
        }

        // POST: VanbanDen/Process/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int maVanBanDen, int trangThai, string nhanXet, List<string> nguoiNhanIds)
        {
            // Kiểm tra trạng thái trước khi xử lý
            var lichSuXuLy = await _context.LichSuXuLys
                .Include(l => l.NguoiDuyet)
                    .ThenInclude(n => n.ChucVu)
                .FirstOrDefaultAsync(l => l.MaVanBanDen == maVanBanDen && l.MaNguoiDuyet == CurrentUserID);

            if (lichSuXuLy == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu đã duyệt rồi thì không cho phép sửa
            if (lichSuXuLy.TrangThai == 3 || lichSuXuLy.TrangThai == 4)
            {
                TempData["Error"] = "Văn bản này đã được xử lý, không thể thay đổi trạng thái!";
                return RedirectToAction(nameof(Index));
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var vanBanDen = await _context.VanBanDens
                        .Include(v => v.VanBanDi)
                            .ThenInclude(v => v.NguoiGui)
                        .Include(v => v.LichSuXuLy)
                            .ThenInclude(l => l.NguoiDuyet)
                                .ThenInclude(n => n.ChucVu)
                        .FirstOrDefaultAsync(v => v.MaVanBanDen == maVanBanDen);

                    if (vanBanDen == null)
                    {
                        return NotFound();
                    }

                    var currentUserId = CurrentUserID;
                    var buocHienTai = lichSuXuLy.Buoc;
                    var nextStep = buocHienTai + 1;
                    var isLastStep = !await _context.LichSuXuLys.AnyAsync(l => l.MaVanBanDi == vanBanDen.MaVanBanDi && l.Buoc == nextStep);
                    string message;

                    // Cập nhật trạng thái và nhận xét cho lịch sử xử lý hiện tại
                    lichSuXuLy.TrangThai = trangThai;
                    lichSuXuLy.NhanXet = nhanXet;
                    lichSuXuLy.NgayDuyet = DateTime.Now;
                    _context.Update(lichSuXuLy);

                    if (trangThai == 4) // Từ chối
                    {
                        // Cập nhật trạng thái văn bản đến thành từ chối
                        vanBanDen.TrangThai = 4;
                        _context.Update(vanBanDen);

                        // Cập nhật trạng thái văn bản đi thành từ chối
                        vanBanDen.VanBanDi.TrangThai = 4; // Từ chối
                        _context.Update(vanBanDen.VanBanDi);

                        // Thông báo cho người gửi về việc từ chối
                        message = $"Văn bản đã bị từ chối bởi {lichSuXuLy.NguoiDuyet?.HoTen ?? "Không xác định"}. Lý do: {nhanXet}";

                        // Tạo thông báo cho người gửi
                        var thongBaoTuChoi = new ThongBao
                        {
                            NguoiNhan = vanBanDen.VanBanDi.MaNguoiGui,
                            NoiDung = message,
                            NgayTao = DateTime.Now,
                            DaXem = false,
                            LoaiThongBao = 3, // Thông báo từ chối
                            DuongDanVanBan = $"/VanbanDi/Details/{vanBanDen.MaVanBanDi}"
                        };
                        _context.ThongBaos.Add(thongBaoTuChoi);

                        // Cập nhật trạng thái các bước sau thành từ chối
                        var lichSuXuLysSau = await _context.LichSuXuLys
                            .Where(l => l.MaVanBanDi == vanBanDen.MaVanBanDi && l.Buoc > buocHienTai)
                            .ToListAsync();

                        foreach (var lsxl in lichSuXuLysSau)
                        {
                            lsxl.TrangThai = 4; // Từ chối
                            lsxl.NhanXet = "Văn bản đã bị từ chối ở bước trước";
                            lsxl.NgayDuyet = DateTime.Now;
                            _context.Update(lsxl);
                        }
                    }
                    else if (trangThai == 3) // Đã duyệt
                    {
                        if (isLastStep)
                        {
                            if (nguoiNhanIds == null || !nguoiNhanIds.Any())
                            {
                                TempData["Message"] = "Vui lòng chọn ít nhất một người nhận.";
                                return RedirectToAction(nameof(Process), new { id = maVanBanDen });
                            }

                            // Cập nhật trạng thái văn bản đến thành đã duyệt
                            vanBanDen.TrangThai = 3;
                            _context.Update(vanBanDen);

                            // Cập nhật trạng thái văn bản đi thành đã duyệt
                            vanBanDen.VanBanDi.TrangThai = 3;
                            _context.Update(vanBanDen.VanBanDi);

                            // Thông báo cho người gửi về việc duyệt thành công
                            message = $"Văn bản đã được duyệt bởi {lichSuXuLy.NguoiDuyet.HoTen} ({lichSuXuLy.NguoiDuyet.ChucVu.TenChucVu}) và đã được chuyển đến người nhận.";

                            // Tạo thông báo cho người gửi
                            var thongBaoDuyet = new ThongBao
                            {
                                NguoiNhan = vanBanDen.VanBanDi.MaNguoiGui,
                                NoiDung = message,
                                NgayTao = DateTime.Now,
                                DaXem = false,
                                LoaiThongBao = 2,
                                DuongDanVanBan = $"/VanbanDi/Details/{vanBanDen.MaVanBanDi}"
                            };
                            _context.ThongBaos.Add(thongBaoDuyet);

                            // Tạo văn bản đến mới cho những người được chọn (chỉ để xem)
                            foreach (var nguoiNhanId in nguoiNhanIds)
                            {
                                var vanBanDenMoi = new VanBanDen
                                {
                                    MaVanBanDi = vanBanDen.MaVanBanDi,
                                    NguoiNhan = nguoiNhanId,
                                    NgayNhan = DateTime.Now,
                                    TrangThai = 3 // Đã duyệt - chỉ để xem
                                };
                                _context.VanBanDens.Add(vanBanDenMoi);

                                // Tạo thông báo cho người nhận mới
                                var thongBaoNhan = new ThongBao
                                {
                                    NguoiNhan = nguoiNhanId,
                                    NoiDung = $"Bạn có văn bản mới để xem: {vanBanDen.VanBanDi.SoHieuVanBan}",
                                    NgayTao = DateTime.Now,
                                    DaXem = false,
                                    LoaiThongBao = 1,
                                    DuongDanVanBan = $"/VanbanDen/Details/{vanBanDenMoi.MaVanBanDen}"
                                };
                                _context.ThongBaos.Add(thongBaoNhan);
                            }
                        }
                        else
                        {
                            // Cập nhật trạng thái văn bản đến hiện tại thành đã duyệt
                            vanBanDen.TrangThai = 3; // Đã duyệt
                            _context.Update(vanBanDen);

                            // Lấy thông tin người xử lý bước tiếp theo
                            var nextPerson = await _context.LichSuXuLys
                                .Include(l => l.NguoiDuyet)
                                .Include(l => l.NguoiDuyet.ChucVu)
                                .FirstOrDefaultAsync(l => l.MaVanBanDi == vanBanDen.MaVanBanDi && l.Buoc == nextStep);

                            if (nextPerson != null)
                            {
                                // Lấy văn bản đến của người xử lý tiếp theo
                                var nextVanBanDen = await _context.VanBanDens
                                    .FirstOrDefaultAsync(v => v.MaVanBanDi == vanBanDen.MaVanBanDi && 
                                                            v.NguoiNhan == nextPerson.MaNguoiDuyet);

                                if (nextVanBanDen != null)
                                {
                                    // Cập nhật trạng thái văn bản đến của người tiếp theo thành chưa duyệt
                                    nextVanBanDen.TrangThai = 2; // Chưa duyệt
                                    _context.Update(nextVanBanDen);
                                }

                                message = $"Văn bản đã được duyệt bởi {lichSuXuLy.NguoiDuyet.HoTen} ({lichSuXuLy.NguoiDuyet.ChucVu.TenChucVu}) và đã được chuyển đến {nextPerson.NguoiDuyet.HoTen} ({nextPerson.NguoiDuyet.ChucVu.TenChucVu}) để xử lý tiếp theo.";

                                // Tạo thông báo cho người xử lý tiếp theo
                                var thongBaoNext = new ThongBao
                                {
                                    NguoiNhan = nextPerson.MaNguoiDuyet,
                                    NoiDung = $"Bạn có văn bản mới cần xử lý: {vanBanDen.VanBanDi.SoHieuVanBan}",
                                    NgayTao = DateTime.Now,
                                    DaXem = false,
                                    LoaiThongBao = 1, // Thông báo văn bản đến
                                    DuongDanVanBan = $"/VanbanDen/Process/{nextVanBanDen.MaVanBanDen}"
                                };
                                _context.ThongBaos.Add(thongBaoNext);

                                // Cập nhật trạng thái văn bản đi thành đang xử lý
                                vanBanDen.VanBanDi.TrangThai = 2; // Đang xử lý
                                _context.Update(vanBanDen.VanBanDi);
                            }
                            else
                            {
                                message = $"Văn bản đã được duyệt bởi {lichSuXuLy.NguoiDuyet.HoTen} ({lichSuXuLy.NguoiDuyet.ChucVu.TenChucVu}).";
                            }

                            // Tạo thông báo cho người gửi
                            var thongBaoDuyet = new ThongBao
                            {
                                NguoiNhan = vanBanDen.VanBanDi.MaNguoiGui,
                                NoiDung = message,
                                NgayTao = DateTime.Now,
                                DaXem = false,
                                LoaiThongBao = 2, // Thông báo duyệt
                                DuongDanVanBan = $"/VanbanDi/Details/{vanBanDen.MaVanBanDi}"
                            };
                            _context.ThongBaos.Add(thongBaoDuyet);
                        }
                    }
                    else
                    {
                        message = $"Văn bản đã được cập nhật trạng thái bởi {lichSuXuLy.NguoiDuyet?.HoTen ?? "Không xác định"} " +
                            $"({lichSuXuLy.NguoiDuyet?.ChucVu?.TenChucVu ?? "Không xác định"}).";
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Message"] = message;

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Có lỗi xảy ra khi xử lý văn bản: " + ex.Message);
                    return RedirectToAction(nameof(Process), new { id = maVanBanDen });
                }
            }
        }

        // GET: VanbanDen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vanbanDen = await _context.VanBanDens
                .FirstOrDefaultAsync(m => m.MaVanBanDen == id);
            if (vanbanDen == null)
            {
                return NotFound();
            }
            return View(vanbanDen);
        }

        // GET: VanbanDen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VanbanDen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVanBanDi,NguoiNhan,TrangThai")] VanBanDen vanbanDen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vanbanDen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vanbanDen);
        }

        // GET: VanbanDen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var vanbanDen = await _context.VanBanDens.FindAsync(id);
            if (vanbanDen == null)
            {
                return NotFound();
            }
            return View(vanbanDen);
        }

        // POST: VanbanDen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaVanBanDen,MaVanBanDi,NguoiNhan,TrangThai")] VanBanDen vanbanDen)
        {
            if (id != vanbanDen.MaVanBanDen)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vanbanDen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VanbanDenExists(vanbanDen.MaVanBanDen))
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
            return View(vanbanDen);
        }

        //// GET: VanbanDen/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var vanbanDen = await _context.VanBanDens
        //        .FirstOrDefaultAsync(m => m.MaVanBanDen == id);
        //    if (vanbanDen == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(vanbanDen);
        //}

        //// POST: VanbanDen/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var vanbanDen = await _context.VanBanDens.FindAsync(id);
        //    _context.VanBanDens.Remove(vanbanDen);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool VanbanDenExists(int id)
        {
            return _context.VanBanDens.Any(e => e.MaVanBanDen == id);
        }
    }
} 