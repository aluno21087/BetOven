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
    // todos os métodos desta classe ficam protegidos. Só pessoas autorizadas têm acesso.
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

        /// <summary>
        /// variável que tem o objetivo de recolher os dados de uma pessoa que está autenticada
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilizadoresController(BetOvenDB context, IWebHostEnvironment caminho, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _caminho = caminho;
            _userManager = userManager;
        }

        // GET: Utilizadores
        /// <summary>
        /// lista os dados de um Utilizador no ecrã
        /// um utilizador não autenticado poderá ver este ecrã
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous] // este anotador anula o efeito da restrição imposta pelo [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                ViewBag.Saldo = util.Saldo;

                return View(await _context.Utilizadores.ToListAsync());
            }
            catch { return View(await _context.Utilizadores.ToListAsync()); }

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
                //caso o id seja nulo retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }
            // SELECT * FROM Users WHERE Users.UserId = id
            var users = await _context.Utilizadores.Include(a => a.ListaApostas).Include(a => a.ListaDepositos).Where(u => u.UserId == id)
                                                    .FirstOrDefaultAsync();
            if (users == null)
            {
                //caso nao seja possivel aceder a um utilizador retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }

                //para poder ver a ViewBag com o Saldo do Utilizador
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                ViewBag.Saldo = util.Saldo;

                // retorna a vista dos Utilizadores
                return View(users);

        }

        // GET: Users/Create
        /// <summary>
        /// Criação de um Utilizador
        /// Apenas é permitido a execução desta tarefa aos Utilizadores com roles Administrativos
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        /// <summary>
        /// a criação de um utilzador requer alguns atributos
        /// nomeadamente:
        /// nome, email, nickname, nacionalidade, data de nascimento, saldo inicial, fotografia
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fotoUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Create([Bind("UserID, Nome, Email, Nickname, Nacionalidade, Datanasc, Saldo, Fotografia")] Utilizadores user, IFormFile fotoUser)
        {
            // variáveis auxiliares
            string caminhoCompleto = "";
            bool haImagem = false;

            // será que há fotografia?
            //    - caso não exista, é adicionada uma imagem "por defeito" que será igual para todos os utilizadores
            //      que não possuam uma fotografia exemplificativa 
            if (fotoUser == null) { user.Fotografia = "noUser.png"; }
            else
            {
                //as extensões aceites são ".jpeg"; ".jpg" e ".png"
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
                    // então coloca-se a imagem "padrão"
                    user.Fotografia = "noUser.png";
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    // se há imagem, vou guardá-la no disco rígido
                    if (haImagem)
                    {
                        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                        await fotoUser.CopyToAsync(stream);
                    }
                    return RedirectToAction(nameof(Index));
                }

                // caso este catch seja executado, houve algo que correu mal no processo
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        /// <summary>
        /// edição dos dados de um utilizador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }

            var users = await _context.Utilizadores.FindAsync(id);
            if (users == null)
            {
                //caso nao seja possivel identificar o utilizador retorna-se à página Index dos Jogos
                return RedirectToAction("Index", "Utilizadores");
            }
            return View(users);
        }

        // POST: Users/Edit/5
        /// <summary>
        /// serve para editar os dados de um utilzador
        /// não é apenas permitido aos administradores uma vez
        /// que os utilizadores têm de ter esse direito
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <param name="fotoUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Nome,Email,Nickname,Nacionalidade,Datanasc,Saldo,Fotografia")] Utilizadores user, IFormFile fotoUser)
        {
            if (id != user.UserId)
            {
                //caso o id não seja igual retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }

            // variáveis auxiliares
            string caminhoCompleto = "";
            bool haImagem = false;

            // o mesmo processo da criação de utilizadores aplica-se aqui na edição 
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

                    // se há imagem, vou guardá-la no disco rígido
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
                        //se chegarmos a este ponto sem sucesso, retorna-se à página Index dos Utilizadores
                        return RedirectToAction("Index", "Utilizadores");
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

        // GET: Users/Delete/
        /// <summary>
        /// Eliminação de Utilizadores
        /// Apenas é permitida a execução desta tarefa aos Utilizadores com roles Administrativas
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }

            var users = await _context.Utilizadores
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                //caso nao seja possivel aceder ao jogo retorna-se à página Index dos Utilizadores
                return RedirectToAction("Index", "Utilizadores");
            }

            return View(users);
        }

        // POST: Users/Delete/5
        /// <summary>
        /// serve para apagar um utilizador 
        /// um utilizador nao autorizado nao o poderá fazer uma vez que 
        /// não cumpre as regras gerais de um projeto com autenticação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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
