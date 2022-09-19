using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;

namespace TiendaVirtual.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly DatosTienda _context;

        public CategoriasController(DatosTienda context)
        {
            _context = context;
        }

        public async Task<IActionResult> InicioCat()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        public IActionResult CreateCate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCate(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(InicioCat));
                }


                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe una categoria con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }

            }
            return View(categoria);

        }

        public async Task<IActionResult> EditCate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCate(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(InicioCat));

                }


                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe una categoria con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }

            }
            return View(categoria);
        }

        public async Task<IActionResult> DetailsCate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(e => e.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        public async Task<IActionResult> DeleteCate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categoria = await _context.Categorias.FirstOrDefaultAsync(e => e.Id == id);
            if (categoria == null)
            {
                NotFound();
            }
            return View();
        }

        // POST: Estadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCate(int id)
        {
            if (_context.Categorias == null)
            {
                return Problem("Entity set 'MiTiendaContext.Paises'  is null.");
            }
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InicioCat));
        }
    }
}
