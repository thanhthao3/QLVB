using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class DonViController : Controller
    {
        private readonly QuanLyVanBangDBContext _context;

        public DonViController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: DonVi
        public async Task<IActionResult> Index()
        {
            return View(await _context.DonVis.ToListAsync());
        }

        // GET: DonVi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var donVi = await _context.DonVis
                .FirstOrDefaultAsync(m => m.MaDonVi == id);
            if (donVi == null)
            {
                return NotFound();
            }
            return View(donVi);
        }

        // GET: DonVi/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DonVi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenDonVi,KiHieu,MoTa")] DonVi donVi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donVi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donVi);
        }

        // GET: DonVi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var donVi = await _context.DonVis.FindAsync(id);
            if (donVi == null)
            {
                return NotFound();
            }
            return View(donVi);
        }

        // POST: DonVi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDonVi,TenDonVi,KiHieu,MoTa")] DonVi donVi)
        {
            if (id != donVi.MaDonVi)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donVi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonViExists(donVi.MaDonVi))
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
            return View(donVi);
        }

        // GET: DonVi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var donVi = await _context.DonVis
                .FirstOrDefaultAsync(m => m.MaDonVi == id);
            if (donVi == null)
            {
                return NotFound();
            }
            return View(donVi);
        }

        // POST: DonVi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donVi = await _context.DonVis.FindAsync(id);
            _context.DonVis.Remove(donVi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonViExists(int id)
        {
            return _context.DonVis.Any(e => e.MaDonVi == id);
        }
    }
}
