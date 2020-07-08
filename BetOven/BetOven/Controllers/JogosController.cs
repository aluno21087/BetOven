using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BetOven.Data;
using BetOven.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BetOven.Controllers
{
    public class JogosController : Controller
    {
        private readonly BetOvenDB _context;

        /// <summary>
        /// variável que contém os dados do 'ambiente' do servidor. 
        /// Em particular, onde estão os ficheiros guardados, no disco rígido do servidor
        /// </summary>
        private readonly IWebHostEnvironment _caminho;

        public JogosController(BetOvenDB context, IWebHostEnvironment caminho)
        {
            _context = context;
            _caminho = caminho;
        }

        // GET: Jogos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Jogos.ToListAsync());
        }

        // GET: Jogos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogos = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Njogo == id);
            if (jogos == null)
            {
                return NotFound();
            }

            return View(jogos);
        }

        // GET: Jogos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jogos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Njogo,EquipaA,FotografiaA,EquipaB,FotografiaB,Resultado,Datainiciojogo")] Jogos jogo, IFormFile fotoTeamA, IFormFile fotoTeamB)
        {
            // variáveis auxiliares
            string caminhoCompleto = "";
            bool haImagem = false;

            if (fotoTeamA == null) { jogo.FotografiaA = "noTeam.jpg"; }
            else
            {
                if (fotoTeamA.ContentType == "image/jpeg" || fotoTeamA.ContentType == "image/jpg" || fotoTeamA.ContentType == "image/png")
                {
                    // o ficheiro é uma imagem válida
                    // preparar a imagem para ser guardada no disco rígido
                    // e o seu nome associado à equipa A
                    Guid g;
                    g = Guid.NewGuid();
                    string extensao = Path.GetExtension(fotoTeamA.FileName).ToLower();
                    string nomeA = g.ToString() + extensao;

                    // onde guardar o ficheiro
                    caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens", nomeA);

                    // associar o nome do ficheiro à equipaA 
                    jogo.FotografiaA = nomeA;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    jogo.FotografiaA = "noTeam.png";
                }

            }

            if (fotoTeamB == null) { jogo.FotografiaB = "noTeam.jpg"; }
            else
            {
                if (fotoTeamB.ContentType == "image/jpeg" || fotoTeamB.ContentType == "image/jpg" || fotoTeamB.ContentType == "image/png")
                {
                    // o ficheiro é uma imagem válida
                    // preparar a imagem para ser guardada no disco rígido
                    // e o seu nome associado à equipa A
                    Guid g;
                    g = Guid.NewGuid();
                    string extensao = Path.GetExtension(fotoTeamA.FileName).ToLower();
                    string nomeB = g.ToString() + extensao;

                    // onde guardar o ficheiro
                    caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens", nomeB);

                    // associar o nome do ficheiro à equipaA 
                    jogo.FotografiaB = nomeB;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    jogo.FotografiaB = "noTeam.png";
                }

            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(jogo);
                    await _context.SaveChangesAsync();
                    if (haImagem)
                    {
                        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                        await fotoTeamA.CopyToAsync(stream);
                        await fotoTeamB.CopyToAsync(stream);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }

                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        // GET: Jogos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogos = await _context.Jogos.FindAsync(id);
            if (jogos == null)
            {
                return NotFound();
            }
            return View(jogos);
        }

        // POST: Jogos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Njogo,EquipaA,EquipaB,Resultado,Datainiciojogo")] Jogos jogos)
        {
            if (id != jogos.Njogo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogosExists(jogos.Njogo))
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
            return View(jogos);
        }

        // GET: Jogos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogos = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Njogo == id);
            if (jogos == null)
            {
                return NotFound();
            }

            return View(jogos);
        }

        // POST: Jogos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogos = await _context.Jogos.FindAsync(id);
            _context.Jogos.Remove(jogos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JogosExists(int id)
        {
            return _context.Jogos.Any(e => e.Njogo == id);
        }
    }
}
