using System;
using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;

namespace SuperShop.Data
{
    //objectivo: se n existir BD -> qd corro o programa é criada uma -> tem de ser aplicado no Program e no startup
    //Dá mt jeito para qd se apaga a BD -> qd se volta a correr -> aparece uma nova
    public class SeedDb
    {
        #region Propriedades
        private readonly DataContext _context;
        private Random _random; //gera os produtos aleatóriamente
        #endregion

        public SeedDb(DataContext context)
        {
            _context = context;
            _random = new Random();
        }

        //método q vai criar o seed de forma assincrona
        public async Task SeedAsync()
        {
            //vai criar a BD -> se a BD j estiver criada -> continua
            await _context.Database.EnsureCreatedAsync();

            //inserir os produtos na tabela da BD
            //se n existirem produtos -> usa o método para adicioanar os produtos -> adiciona smp estes
            if(!_context.Products.Any())
            {
                AddProduct("iPhone X"); //adiciona em memória cada produto
                AddProduct("Magic Mouse");
                AddProduct("iWatch Serires 4");
                AddProduct("iPad Mini");

                await _context.SaveChangesAsync(); //adiciona à base de dados
            }
        }

        //método q vai criar os produtos
        private void AddProduct(string name)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000), //cria um preço aleatório até 1000
                IsAvailable = true,
                Stock = _random.Next(100)
            });
        }
    }
}
