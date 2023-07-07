using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //configura��o do user -> usa a minha identidade User e o IdentityRole 
            //configurar a pass -> neste caso sem protec��o
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DataContext>(); //dp do servi�o estar implementado -> dp do login -> volta a usar o datacontext simples

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = this.Configuration["Tokens:Issuer"],
                        ValidAudience = this.Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                    };
                });


            //configurar o datacontext -> cahmar o servi�o -> usar o sql com essa connectionstring
            services.AddDbContext<DataContext>(cfg =>
            {
                //estipular o tipo de base de dados 
                // instalar nuget Microsoft.EntityFrameworkCore.SqlServer
                //ir buscar a mnh configura��o -> ou seja o appsettingjson -> DfaultConnection
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
;
            });

            //usar o servi�o do seedDb para criar a BD se n existir
            //addtransient -> usa e deita fora -> deixa de ficar em mem�ria. Pq qd for usado -> passamos a ter BD
            services.AddTransient<SeedDb>();

            services.AddScoped<IUserHelper, UserHelper>();

            services.AddScoped<IBlobHelper, BlobHelper>();

            services.AddScoped<IConverterHelper, ConverterHelper>();

            // AddScope ->qq servi�o/objecto q apare�a, fica criado e instanciado -> qd crio outro do mm tipo -> apaga o anterior e fica com o novo
            //Assim q detectar q � preciso um repositorio -> cria um
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICountryRepository, CountryRepository>();


            // AddSingleton -> o objecto nc � destruido -> fica smp em mem�ria ->>>> OCUPA mt mem�ria

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized"; //anula a action de retorno 
                options.AccessDeniedPath = "/Account/NotAuthorized"; //chama esta action view
            });



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
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

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
