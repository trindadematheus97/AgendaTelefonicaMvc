using AgendaTelefonica.Context;
using AgendaTelefonica.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaTelefonica.Controllers
{
    public class PessoasController : Controller
    {
        private readonly AgendaContext _agendaContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PessoasController(AgendaContext agendaContext, IWebHostEnvironment webHostEnvironment)
        {
            _agendaContext = agendaContext;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _agendaContext.Pessoas.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> CriarPessoa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarPessoa(Pessoa pessoa, IFormFile foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string diretorio = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    using (FileStream fileStream = new FileStream(Path.Combine(diretorio, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileStream);
                        pessoa.Foto = "~/img/" + nomeFoto;
                    }
                }
                else
                {
                    pessoa.Foto = "~/img/usuario.png";
                }
                _agendaContext.Add(pessoa);
                await _agendaContext.SaveChangesAsync();
                TempData["NovoContato"] = $"Contato com nome {pessoa.Nome} {pessoa.Sobrenome} incluído com sucesso";

                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }


        [HttpGet]
        public async Task<IActionResult> AtualizarPessoa(int pessoaId)
        {
            Pessoa pessoa = await _agendaContext.Pessoas.FindAsync(pessoaId);

            if (pessoa == null)
                return NotFound();

            TempData["Foto"] = pessoa.Foto;
            return View(pessoa);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarPessoa(Pessoa pessoa, IFormFile foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null)
                {
                    string diretorio = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string nomeFoto = Guid.NewGuid().ToString() + foto.FileName;

                    using (FileStream fileStream = new FileStream(Path.Combine(diretorio, nomeFoto), FileMode.Create))
                    {
                        await foto.CopyToAsync(fileStream);
                        pessoa.Foto = "~/img/" + nomeFoto;
                        TempData["Foto"] = null;
                    }
                }
                else
                {
                    pessoa.Foto = TempData["Foto"].ToString();
                }

                _agendaContext.Update(pessoa);
                await _agendaContext.SaveChangesAsync();
                TempData["ContatoAtualizado"] = $"Contato com nome {pessoa.Nome} {pessoa.Sobrenome} atualizado com sucesso";
                return RedirectToAction(nameof(Index));
            }

            return View(pessoa);
        }

        [HttpPost]
        public async Task<JsonResult> ExcluirPessoa(int pessoaId)
        {
            Pessoa pessoa = await _agendaContext.Pessoas.FindAsync(pessoaId);
            _agendaContext.Pessoas.Remove(pessoa);
            await _agendaContext.SaveChangesAsync();
            TempData["ContatoExcluido"] = $"Contato com nome {pessoa.Nome} {pessoa.Sobrenome} excluído com sucesso";
            return Json(true);
        }
    }
}
