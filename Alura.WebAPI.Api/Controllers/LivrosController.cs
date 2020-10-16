using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    //[Route("api/[controller]")] - Versionamento por Query string ou Cabeçalho da Requisicao
    [Route("api/v{version:ApiVersion}/[controller]")] // Versionamento por URI
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retorna uma lista de livros conforme parametros",
                           Produces = new[] { "application/json", "application/xml" })]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToApi()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retorna o livro identificado por seu id.",
                          Tags = new[] { "Livros" },
                          Produces = new[] { "application/json", "application/xml" })]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroApi))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        [ProducesResponseType(statusCode: 404)]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToApi());
        }

        [HttpGet("{id}/capa")]
        [SwaggerOperation(Summary = "Recupera imagem de capa do livro identificado por seu {id}.",
                          Tags = new[] { "Livros" },
                           Produces = new[] { "application/json", "application/xml" })]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Registro novo livro na base ",
                                      Tags = new[] { "Livros" },
                          Produces = new[] { "application/json", "application/xml" })]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro); //201
            }
            return BadRequest();
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Atualiza informações do livro",
                                      Tags = new[] { "Livros" },
                           Produces = new[] { "application/json", "application/xml" })]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok(); //200
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remove livro identificado pelo ID",
                                      Tags = new[] { "Livros" },
                           Produces = new[] { "application/json", "application/xml" })]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent(); //203
        }
    }
}