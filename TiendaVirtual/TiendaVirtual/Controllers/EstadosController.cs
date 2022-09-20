using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVirtual.Data;
using TiendaVirtual.Data.Entities;
using TiendaVirtual.Models;

namespace TiendaVirtual.Controllers
{
    //Este authorize especifica que el unico que va tener permiso para acceder a ubicaciones, para
    //crear, editar, eliminar es el administrador. 
    [Authorize(Roles ="Admin")]
    public class EstadosController : Controller
    {
        
        private readonly DatosTienda _context;

        //Se inyecta el DatosTienda por medio del constructor.
        public EstadosController(DatosTienda context)
        {
            _context = context;
        }

       //Estos métodos son asincronos, todo método asincrono debe ir acompañado
       //de un await, el await lo que hace es esperar que termine la consulta para poder ejecutarla.
        public async Task<IActionResult> Inicio()
        {
            return View(await _context.Estados
                .Include(e => e.Ciudades)
                .ToListAsync());
        }

  

 
        public IActionResult Create()
        {
            Estado estado = new() { Ciudades = new List<Ciudad>()}; 
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Estado estado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(estado);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Inicio));
                }
                  

                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe un departamento/estado con el mismo nombre.");
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
            return View(estado);
        }

        public async Task<IActionResult> CreateCiudad(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

                Estado estado = await _context.Estados.FindAsync(id);
                if(estado == null)
                {
                    NotFound();
                }
                CiudadVistaModelo model = new()
                {
                    EstadoId = estado.Id,
                };
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCiudad(CiudadVistaModelo model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Ciudad ciudad = new()
                    {
                        Estado = await _context.Estados.FindAsync(model.EstadoId),
                        Nombre = model.Nombre,
                    };

                    _context.Add(ciudad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {Id = model.EstadoId});
                }


                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe una ciudad con el mismo nombre.");
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
            return View(model);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.Estados
                .Include(c => c.Ciudades)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (estado == null)
            {
                return NotFound();
            }
            
            return View(estado);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Estado estado)
        {
            if (id != estado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estado);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Inicio));
                }


                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe un departamento/estado con el mismo nombre.");
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
            return View(estado);
        }

        public async Task<IActionResult> EditCiudad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudad = await _context.Ciudades
                .Include(c => c.Estado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (ciudad == null)
            {
                return NotFound();
            }
            CiudadVistaModelo model = new()
            {
                EstadoId = ciudad.Estado.Id,
                Id = ciudad.Id,
                Nombre = ciudad.Nombre,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCiudad(int id, CiudadVistaModelo model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Ciudad ciudad = new()
                    {
                        Id = model.Id,
                        Nombre = model.Nombre,
                    };
                    _context.Update(ciudad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {Id = model.EstadoId});
                }


                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "ya existe una ciudad con el mismo nombre en este departamento/estado.");
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
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.Estados
                .Include(e => e.Ciudades)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (estado == null)
            {
                return NotFound();
            }

            return View(estado);
        }

        public async Task<IActionResult> DetailsCiudad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ciudad = await _context.Ciudades
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (ciudad == null)
            {
                return NotFound();
            }

            return View(ciudad);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var estado = await _context.Estados
                .Include(e => e.Ciudades)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (estado == null)
            {
                NotFound();
            }
            return View(estado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Estados == null)
            {
                return Problem("Entity set 'MiTiendaContext.Paises'  is null.");
            }
            var estado = await _context.Estados.FindAsync(id);
            if (estado != null)
            {
                _context.Estados.Remove(estado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Inicio));
        }

        public async Task<IActionResult> DeleteCiudad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ciudad = await _context.Ciudades
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (ciudad == null)
            {
                NotFound();
            }
            return View(ciudad);
        }

        [HttpPost, ActionName("DeleteCiudad")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCiudad(int id)
        {
            
            var ciudad = await _context.Ciudades
                .Include(e => e.Estado)
                .FirstOrDefaultAsync(e => e.Id == id);
                _context.Ciudades.Remove(ciudad);
            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new {Id = ciudad.Estado.Id});
        }


    }
}
