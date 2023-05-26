using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop
{
    //arrabca com as configurações do startup -> é od ficam as configurações
    public class Program
    {
        //host permite adaptar-se a qq sistema
        public static void Main(string[] args)
        {
            /*CreateHostBuilder(args).Build().Run(); */ //cria o host -> o build e corre

            var host = CreateHostBuilder(args).Build(); //constroi o host
            RunSeeding(host);//correr o seed no host q foi criado
            host.Run();//corre o host

        }

        //usa o design patter Factory -> o objecto antes de existir -> cria-se a ele próprio
        private static void RunSeeding(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using(var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<SeedDb>();
                seeder.SeedAsync().Wait();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
