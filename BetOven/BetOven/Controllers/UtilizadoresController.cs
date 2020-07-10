using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BetOven.Data;
using BetOven.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BetOven.Controllers
{
    [Authorize]
    public class UtilizadoresController : Controller
    {
        /// <summary>
        /// variável que identifica a BD do nosso projeto
        /// </summary>
        private readonly BetOvenDB _context;

        /// <summary>
        /// variável que contém os dados do 'ambiente' do servidor. 
        /// Em particular, onde estão os ficheiros guardados, no disco rígido do servidor
        /// </summary>
        private readonly IWebHostEnvironment _caminho;

        private readonly UserManager<ApplicationUser> _userManager;

        public UtilizadoresController(BetOvenDB context, IWebHostEnvironment caminho, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _caminho = caminho;
            _userManager = userManager;
        }

        // GET: Utilizadores
        [AllowAnonymous] // este anotador anula o efeito da restrição imposta pelo [Authorize]
        public async Task<IActionResult> Index()
        {
            // SELECT * FROM Utilizadores
            return View(await _context.Utilizadores.ToListAsync());
        }

        // GET: Users/Details/5
        /// <summary>
        /// Mostra os detalhes de um User
        /// Se houverem, então mostra os detalhes dos depósitos associados ao mesmo
        /// Pesquisa em 'Lazy Loading'
        /// </summary>
        /// <param name="id">Identificação do User</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // SELECT * FROM Users WHERE Users.UserId = id
            var users = await _context.Utilizadores.Where( u => u.UserId == id )
                                                   .FirstOrDefaultAsync();
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        //[Authorize(Roles = "Administrativo")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Create([Bind("UserID, Nome, Email, Nickname, Nacionalidade, Datanasc, Saldo, Fotografia")] Utilizadores user, IFormFile fotoUser)
        {
            // variáveis auxiliares
            string caminhoCompleto = "";
            bool haImagem = false;

            if (fotoUser == null) { user.Fotografia = "noUser.png"; }
            else
            {
                if (fotoUser.ContentType=="image/jpeg" || fotoUser.ContentType == "image/jpg" || fotoUser.ContentType == "image/png")
                {
                    // o ficheiro é uma imagem válida
                    // preparar a imagem para ser guardada no disco rígido
                    // e o seu nome associado ao Utilizador
                    Guid g;
                    g = Guid.NewGuid();
                    string extensao = Path.GetExtension(fotoUser.FileName).ToLower();
                    string nome = g.ToString() + extensao;

                    // onde guardar o ficheiro
                    caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens", nome);

                    // associar o nome do ficheiro ao Utilizador 
                    user.Fotografia = nome;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    user.Fotografia = "noUser.png";
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    if (haImagem)
                    {
                        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                        await fotoUser.CopyToAsync(stream);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Utilizadores.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Nome,Email,Nickname,Nacionalidade,Datanasc,Saldo,Fotografia")] Utilizadores user, IFormFile fotoUser)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // variáveis auxiliares
            string caminhoCompleto = "";
            bool haImagem = false;

            if (fotoUser == null) { user.Fotografia = "noUser.png"; }
            else
            {
                if (fotoUser.ContentType == "image/jpeg" || fotoUser.ContentType == "image/jpg" || fotoUser.ContentType == "image/png")
                {
                    // o ficheiro é uma imagem válida
                    // preparar a imagem para ser guardada no disco rígido
                    // e o seu nome associado ao Utilizador
                    Guid g;
                    g = Guid.NewGuid();
                    string extensao = Path.GetExtension(fotoUser.FileName).ToLower();
                    string nome = g.ToString() + extensao;

                    // onde guardar o ficheiro
                    caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens", nome);

                    // associar o nome do ficheiro ao Utilizador 
                    user.Fotografia = nome;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    user.Fotografia = "noUser.png";
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    if (haImagem)
                    {
                        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                        await fotoUser.CopyToAsync(stream);
                    }
                }

                
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Utilizadores.FindAsync(id);
            _context.Utilizadores.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Utilizadores.Any(e => e.UserId == id);
        }
    }
}
