using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //configurar o datacontext -> cahmar o serviço -> usar o sql com essa connectionstring
            services.AddDbContext<DataContext>(cfg =>
            {
                //estipular o tipo de base de dados 
                // instalar nuget Microsoft.EntityFrameworkCore.SqlServer
                //ir buscar a mnh configuração -> ou seja o appsettingjson -> DfaultConnection
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
;
            });

            //usar o serviço do seedDb para criar a BD se n existir
            //addtransient -> usa e deita fora -> deixa de ficar em memória. Pq qd for usado -> passamos a ter BD
            services.AddTransient<SeedDb>();

            // AddScope ->qq serviço/objecto q apareça, fica criado e instanciado -> qd crio outro do mm tipo -> apaga o anterior e fica com o novo
            //Assim q detectar q é preciso um repositorio -> cria um
            services.AddScoped<IProductRepository, ProductRepository>(); 

            // AddSingleton -> o objecto nc é destruido -> fica smp em memória ->>>> OCUPA mt memória




            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
