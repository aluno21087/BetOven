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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BetOven.Controllers
{
    //Authorização refere-se como um processo que determina o que um utilizador é capaz de fazer
    //No caso dos Jogos, apenas seria competente um utilizador autorizado conseguir ver que tipos de jogos existem para apostar
    [Authorize]
    public class JogosController : Controller
    {
        /// <summary>
        /// esta é a variável que identifica a nossa Base de Dados do projeto
        /// </summary>
        private readonly BetOvenDB _context;

        /// <summary>
        /// variável que tem o objetivo de recolher os dados de uma pessoa que está autenticada
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// variável que contém os dados do 'ambiente' do servidor. 
        /// Em particular, onde estão os ficheiros guardados, no disco rígido do servidor
        /// </summary>
        private readonly IWebHostEnvironment _caminho;

        public JogosController(BetOvenDB context, IWebHostEnvironment caminho, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _caminho = caminho;
            _userManager = userManager;
        }

        // GET: Jogos
        /// <summary>
        /// lista os dados dos jogos no ecrã
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            return View(await _context.Jogos.ToListAsync());
        }

        // GET: Jogos/Details/5
        /// <summary>
        /// mostra os detalhes de um jogo
        /// mostra:
        /// as fotos de ambas as equipas;
        /// o nome de ambas as equipas;
        /// o resultado do jogo, caso tenha terminado;
        /// data de início do jogo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }

            var jogos = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Njogo == id);

            if (jogos == null)
            {
                //caso nao seja possivel encontrar um "jogo" retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }
            //para poder ver a ViewBag com o Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;

            //retorna a vista dos Jogos
            return View(jogos);
        }

        // GET: Jogos/Create
        /// <summary>
        /// a criação de um jogo cabe aos administradores
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jogos/Create
        /// <summary>
        /// serve para criar um jogo por parte do Administrador
        /// para tal se suceder é necessário:
        /// o nome da equipa A
        /// o nome da equipa B
        /// foto da equipa A
        /// foto da equipa B
        /// data do início do evento
        /// </summary>
        /// <param name="jogo"></param>
        /// <param name="fotoTeamA"></param>
        /// <param name="fotoTeamB"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Create([Bind("Njogo,EquipaA,FotoA,EquipaB,FotoB,Resultado,Datainiciojogo")] Jogos jogo, IFormFile fotoTeamA, IFormFile fotoTeamB)
        {
            // variáveis auxiliares
            string caminhoCompletoA = "";
            string caminhoCompletoB = "";
            bool haImagem = false;

            // será que há fotografia?
            //    - caso não exista, é adicionada uma imagem "por defeito" que será igual para todas as equipas
            //      que não possuam uma fotografia exemplificativa 
            if (fotoTeamA == null) { jogo.FotoA = "noTeam.jpg"; }
            else
            {
                //as extensões aceites são ".jpeg"; ".jpg" e ".png"
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
                    caminhoCompletoA = Path.Combine(_caminho.WebRootPath, "Imagens\\Equipas\\", nomeA);

                    // associar o nome do ficheiro à equipaA 
                    jogo.FotoA = nomeA;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    // então coloca-se a imagem "padrão"
                    jogo.FotoA = "noTeam.png";
                }

            }

            // o mesmo processo só que desta vez para a equipa B
            if (fotoTeamB == null) { jogo.FotoB = "noTeam.jpg"; }
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
                    caminhoCompletoB = Path.Combine(_caminho.WebRootPath, "Imagens\\Equipas\\", nomeB);

                    // associar o nome do ficheiro à equipaA 
                    jogo.FotoB = nomeB;

                    // assinalar que existe imagem e é preciso guardá-la no disco rígido
                    haImagem = true;
                }
                else
                {
                    // há imagem, mas não é do tipo correto
                    jogo.FotoB = "noTeam.png";
                }

            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(jogo);
                    await _context.SaveChangesAsync();

                    // se há imagem, vou guardá-la no disco rígido
                    if (haImagem)
                    {
                        using var streamA = new FileStream(caminhoCompletoA, FileMode.Create);
                        using var streamB = new FileStream(caminhoCompletoB, FileMode.Create);
                        await fotoTeamA.CopyToAsync(streamA);
                        await fotoTeamB.CopyToAsync(streamB);
                    }
                    return RedirectToAction(nameof(Index));
                }

                // caso este catch seja executado, houve algo que correu mal no processo
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }

                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        // GET: Jogos/Edit/5
        /// <summary>
        /// edição dos dados de um jogo por parte do Administrador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }

            var jogos = await _context.Jogos.FindAsync(id);
            if (jogos == null)
            {
                //caso nao seja possivel identificar o jogo retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }
            return View(jogos);
        }

        // POST: Jogos/Edit/5
        /// <summary>
        /// apenas um administrador poderá editar os detalhes de um jogo
        /// é também através da edição que um administrador coloca o resultado 
        /// do jogo/evento, por essa razão apenas cabe ao admin fazer a edição
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jogos"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int id, [Bind("Njogo,EquipaA,FotoA,EquipaB,FotoB,Resultado,Datainiciojogo")] Jogos jogos)
        {
            if (id != jogos.Njogo)
            {
                //caso o id não seja igual retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // após a edição de um jogo e ser colocado o resultado do mesmo é necessário tomar 
                    // medidas para que quem apostou corretamente no resultado de um jogo seja recompensado
                    var tree = await _context.Jogos.Include(a => a.ListaApostas).FirstOrDefaultAsync(j => j.Njogo == jogos.Njogo);

                    tree.Resultado = jogos.Resultado;

                    //caso queira alterar um atributo
                    //tree.FotoA = jogos.FotoA;

                    foreach (var item in tree.ListaApostas)
                    {
                        if (item.Descricao == jogos.Resultado)
                        {
                            var vencedor = await _context.Utilizadores.FirstOrDefaultAsync(v => v.UserId == item.UserFK);
                            vencedor.Saldo += item.Quantia * item.Multiplicador; //caso o utilizador tenha vencido, este recebe o valor apostado multiplicado pelo bonus (multiplicador)
                            _context.Update(vencedor);
                           // await _context.SaveChangesAsync();
                            item.Estado = "Ganha"; //o estado da aposta fica "Ganha"
                        }
                        else
                        {
                            item.Estado = "Perdida"; //caso tenha perdido a aposta perde o dinheiro e aparece o estado "Perdida"
                        }
                        _context.Update(item);
                        //await _context.SaveChangesAsync();
                    }
                    var user = await _userManager.GetUserAsync(User);
                    var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                    ViewBag.Saldo = util.Saldo;
                    _context.Update(tree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogosExists(jogos.Njogo))
                    {
                        //caso nao seja possivel aceder ao jogo retorna-se à página Index dos Jogos
                        return RedirectToAction("Index", "Jogos");
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
        /// <summary>
        /// apagar um jogo caso ocorra algum problema
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }

            var jogos = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Njogo == id);
            if (jogos == null)
            {
                //caso nao seja possivel aceder ao jogo retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Jogos");
            }

            return View(jogos);
        }

        // POST: Jogos/Delete/5
        /// <summary>
        /// serve para apagar um jogo
        /// apenas um administrador poderá executar tal tarefa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
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
