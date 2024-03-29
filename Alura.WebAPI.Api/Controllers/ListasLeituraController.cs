﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")] - Versionamento por Query string ou Cabeçalho da Requisicao
    [Route("api/v{version:ApiVersion}/[controller]")] // Versionamento por URI
    public class ListasLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        private Lista CriaLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All
                    .Where(l => l.Lista == tipo)
                    .Select(l => l.ToApi())
                    .ToList()
            };
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retorna uma coleção de livros cadastrados em todas as listas de leitura",
                          Tags = new[] { "Listas" },
                           Produces = new[] { "application/json", "application/xml" })]
        public IActionResult TodasListas()
        {
            Lista paraLer = CriaLista(TipoListaLeitura.ParaLer);
            Lista lendo = CriaLista(TipoListaLeitura.Lendo);
            Lista lidos = CriaLista(TipoListaLeitura.Lidos);
            var colecao = new List<Lista> { paraLer, lendo, lidos };
            return Ok(colecao);
        }

        [HttpGet("{tipo}")]
        [SwaggerOperation(Summary = "Retorna uma coleção de livros cadastrados em todas as listas de leitura conforme o tipo de lista de leitura",
                          Tags = new[] { "Listas" },
                          Produces = new[] { "application/json", "application/xml" })]
        public IActionResult Recuperar([FromRoute] [SwaggerParameter("Tipo da Lista a ser obtida")]TipoListaLeitura tipo)
        {
            var lista = CriaLista(tipo);
            return Ok(lista);
        }
    }
}