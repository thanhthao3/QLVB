using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class TaiKhoanController : BaseController
    {
        private readonly QuanLyVanBangDBContext _context;

        public TaiKhoanController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: TaiKhoan/login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("TenDangNhap,MatKhau")] LoginViewModel model)

        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra tên tài khoản
            var loginUser = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.TenDangNhap == model.TenDangNhap);

            if (loginUser == null)
            { 
                ModelState.AddModelError("", "Tên đăng nhập không đúng");
                return View(model);
            }

            // Kiểm tra mật khẩu
            if (!VerifyPassword(loginUser.MatKhau, model.MatKhau))
            {
                ModelState.AddModelError("", "Mật khẩu không đúng");
                return View(model);
            }

            // Lưu trạng thái user
            CurrentUser = loginUser.TenDangNhap;
            CurrentUserID = loginUser.MaNguoiDung;
            RoleUser = loginUser.Role.ToString();


             return RedirectToAction("Index", "Home");


        }

        private bool VerifyPassword(string storedPassword, string inputPassword)
        {
            using var hashMethod = SHA256.Create();
            return Util.Cryptography.VerifyHash(hashMethod, inputPassword, storedPassword);
        }
        // tạo mới một mã người dùng 
        public async Task<string> GenerateNewNguoiDungIdAsync(int maDonVi)
        {
            // Lấy viết tắt của đơn vị
            var donVi = await _context.DonVis
                .Where(dv => dv.MaDonVi == maDonVi)
                .Select(dv => dv.KiHieu)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(donVi))
            {
                throw new Exception("Không tìm thấy đơn vị.");
            }

            // Tìm ID lớn nhất bắt đầu bằng viết tắt của đơn vị (KT001, KT002,...)
            var lastId = await _context.NguoiDungs
                .Where(nd => nd.MaNguoiDung.StartsWith(donVi))
                .OrderByDescending(nd => nd.MaNguoiDung)
                .Select(nd => nd.MaNguoiDung)
                .FirstOrDefaultAsync();

            int newNumber = 1; // Mặc định nếu chưa có bản ghi nào

            if (!string.IsNullOrEmpty(lastId))
            {
                // Tách số cuối từ ID lớn nhất (Ví dụ: KT001 → 1)
                string numberPart = lastId.Substring(donVi.Length);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newNumber = lastNumber + 1;
                }
            }

            // Tạo ID mới theo định dạng KT001, KT002, ...
            string newId = $"{donVi}{newNumber:D4}";
            return newId;
        }



        public IActionResult Register()
        {
            var model = new NguoiDung();
            model.MaNguoiDung = Guid.NewGuid().ToString();
            ViewBag.chucvu = _context.ChucVus.ToList();
            ViewBag.donvi = _context.DonVis.ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("MaNguoiDung,HoTen,Email,NgaySinh,GioiTinh,DiaChi,SDT,MaChucVu,MaDonVi,TenDangNhap,MatKhau,Role")] NguoiDung model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi tại {modelError.Key}: {error.ErrorMessage}");
                    }
                }

            }
            else  
            {

                var existingAccount = _context.NguoiDungs.FirstOrDefault(m => m.TenDangNhap.Equals(model.TenDangNhap));
                if (existingAccount != null)
                {
                    ModelState.AddModelError("", "Tên tài khoản đã tồn tại. Vui lòng chọn tên tài khoản khác.");
                    return View(model);
                }

                // cài đặt mã người dùng 
                string newId = await GenerateNewNguoiDungIdAsync(model.MaDonVi);
                model.MaNguoiDung = newId; ;

                //Mã hóa mật khẩu 
                SHA256 hashMethod = SHA256.Create();
                model.MatKhau = Util.Cryptography.GetHash(hashMethod, model.MatKhau);
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            CurrentUser = "";
            CurrentUserID = "";
            RoleUser = "";
            return RedirectToAction("Login");
        }


        //// GET: TaiKhoan/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var taiKhoan = await _context.NguoiDungs.FindAsync(id);
        //    if (taiKhoan == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(taiKhoan);
        //}

        //// POST: TaiKhoan/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("MaTaiKhoan,TenTaiKhoan,MatKhau")] TaiKhoan taiKhoan)
        //{
        //    if (id != taiKhoan.MaTaiKhoan)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(taiKhoan);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TaiKhoanExists(taiKhoan.MaTaiKhoan))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(taiKhoan);
        //}

        //// GET: TaiKhoan/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var taiKhoan = await _context.TaiKhoan
        //        .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
        //    if (taiKhoan == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(taiKhoan);
        //}

        //// POST: TaiKhoan/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var taiKhoan = await _context.TaiKhoan.FindAsync(id);
        //    if (taiKhoan != null)
        //    {
        //        _context.TaiKhoan.Remove(taiKhoan);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool TaiKhoanExists(int id)
        //{
        //    return _context.TaiKhoan.Any(e => e.MaTaiKhoan == id);
        //}
    }
}
