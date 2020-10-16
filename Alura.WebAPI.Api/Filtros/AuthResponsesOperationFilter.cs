using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Alura.WebAPI.Api.Filtros
{


    /// <summary>
    /// Aplica documentação  a todas as operações que não tiverem um token válido, produzindo como resposta o código 401. (De forma Global) 
    /// A interface IOperationFilter que permite aplicar um código a cada operação sendo documentada.
    ///Em nosso caso iremos adicionar mais um tipo de resposta (401).
    /// </summary>
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new Response { Description = "Unauthorized" });
        }
    }
}
