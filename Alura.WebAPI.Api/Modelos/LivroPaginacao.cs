using Alura.ListaLeitura.Modelos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Modelos
{

    public static class LivroPaginacaoExtensions
    {
        public static LivroPaginado ToLivroPaginado(this IQueryable<LivroApi> query, LivroPaginacao paginacao)
        {
            int totalItens = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)paginacao.Tamanho); //Pegar o maior valor na divisão do total de paginas 

            return new LivroPaginado 
            { 
                Total = totalItens,
                TotalPaginas = totalPaginas,
                NumeroPagina = paginacao.Pagina,
                TamanhoPagina = paginacao.Tamanho,
                Resultado = query.Skip(paginacao.Tamanho * (paginacao.Pagina - 1))
                                 .Take(paginacao.Tamanho).ToList(),
                Anterior = (paginacao.Pagina > 1) ? $"livros?tamanho={paginacao.Pagina - 1}&pagina={paginacao.Tamanho}":"",
                Proximo = (paginacao.Pagina < totalPaginas)? $"livros?tamanho={paginacao.Pagina + 1}&pagina={paginacao.Tamanho}":""
            };
        }
    }

    public class LivroPaginado
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public IList<LivroApi> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }
    }



    public class LivroPaginacao
    {
        public int Tamanho { get; set; } = 1;
        public int Pagina { get; set; } = 25;
    }
}
