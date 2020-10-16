using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.Api.Filtros
{

    /// <summary>
    /// Customizar a documentação gerada pelo SwaggerGen para adicionar ou modificar alguma informação.     
    /// Coloca uma descrição genérica em cada tag usando esse filtro.
    /// </summary>
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new[]
            {
                new Tag {Name = "Livros", Description = "Consulta e mantem os livros"},
                new Tag {Name = "Listas", Description = "Consulta as listas de leitura"}
            };
        }
    }
}
