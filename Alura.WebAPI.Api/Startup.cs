using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alura.ListaLeitura.Api.Formatters;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Filtros;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Alura.WebAPI.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LeituraContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new LivroCsvFormatter());
                options.Filters.Add(typeof(ErrorResponseFilter));
            }).AddXmlSerializerFormatters();

            //Desabilita filtros do ModelState para customizar o retorno de erros deste tipo na API
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("alura-webapi-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman",
                };
            });


            //Versionamento por URL
            services.AddApiVersioning();

            /*
             * Versionamento por query string ou cabeçalho da requisicao
                services.AddApiVersioning(options =>
                {
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader("api-version"), //usada na requisicao via query string uri?api-version=1.0
                        new HeaderApiVersionReader("api-version") //Usada na requisicao no cabeçalho na key api-version => 1.0
                        );
                });
            */

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Livros API Versão 1.0", Description = "Documentação da API", Version = "1.0" });
                c.SwaggerDoc("v2", new Info { Title = "Livros API Versão 2.0", Description = "Documentação da API", Version = "2.0" });

                c.EnableAnnotations();

                //Exibir na documentação da API qual tipo de autenticação necessária para consumi-la.
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme 
                { 
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                    Description = "Autenticação Bearer via JWT"
                });

                /*Essa configuração irá habilitar um botão chamado Authorize em seu Swagger-UI. 
                 * Através dele você (e quem consultar sua documentação!)
                 * poderá autenticar-se na API para realizar testes nas operações documentadas em seguida.*/
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> 
                {
                    {"Bearer", new string[] { } }
                
                });

                /*
                 * irá indicar quais são seus valores válidos. Contudo, observe que esses valores são 0, 1 e 2. 
                 * Irá mostrar os valores descritivos do enumerado (paraLer, lendo e lidos)
                 */
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();

                /*
                 * Aplica documentação  a todas as operações que não tiverem um token válido, produzindo como resposta o código 401. (De forma Global) 
                 * A interface IOperationFilter que permite aplicar um código a cada operação sendo documentada.
                 */
                c.OperationFilter<AuthResponsesOperationFilter>();

                /*
                 * Customiza a documentação gerada pelo SwaggerGen para adicionar ou modificar alguma informação.     
                 * Coloca uma descrição genérica em cada tag usando esse filtro.
                 */
                c.DocumentFilter<TagDescriptionsDocumentFilter>();

            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão 1.0");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Versão 2.0");
                });
        }
    }
}
