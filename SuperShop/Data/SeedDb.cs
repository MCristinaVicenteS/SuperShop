using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop.Data
{
    //objectivo: se n existir BD -> qd corro o programa é criada uma -> tem de ser aplicado no Program e no startup
    //Dá mt jeito para qd se apaga a BD -> qd se volta a correr -> aparece uma nova
    public class SeedDb
    {
        #region Propriedades
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random; //gera os produtos aleatóriamente
        #endregion

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        //método q vai criar o seed de forma assincrona
        public async Task SeedAsync()
        {
            //vai criar a BD -> se a BD j estiver criada -> continua
            await _context.Database.EnsureCreatedAsync();

            //verificar se os roles existem
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            //criar uma lista de cidades q é associado a um país
            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal"
                });

                await _context.SaveChangesAsync();
            }

            //verificar se o user já existe -> o que a aplicação vai criar -> vai ser o Admin
            var user = await _userHelper.GetUserByEmailAsync("rafaasfs@gmail.com");

            //se for nulo -> cria com estes dados
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Rafael",
                    LastName = "Santos",
                    Email = "rafaasfs@gmail.com",
                    UserName = "rafaasfs@gmail.com",
                    PhoneNumber = "212343555",
                    Address = "Rua Jau 33",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                //usa a classe userManager para criar o user por defeito -> recebe 2 parâmetros (user e pass)
                var result = await _userHelper.AddUserAsync(user, "123456");
                
                if(result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not creat user in seeder");
                }

                //adicionar o role q já existe (criado no userHelper) ao role q vou passar
                //vou passar o admin para ficar associado ao user por default -> fica o admin
                await _userHelper.AddUserToRoleAsync(user, "Admin");

                //associar ao admin, um token criado
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                //confrimar o token
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            //confirmar se o user está no role q foi escolhido
            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            //se o user criado n tiver o role escolhido -> cria a associação
            if(!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            //inserir os produtos na tabela da BD
            //se n existirem produtos -> usa o método para adicioanar os produtos -> adiciona smp estes
            //associada a cada produto o user que o criou -> neste caso o user
            if(!_context.Products.Any())
            {
                AddProduct("iPhone X", user); //adiciona em memória cada produto
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Serires 4", user);
                AddProduct("iPad Mini", user);

                await _context.SaveChangesAsync(); //adiciona à base de dados
            }
        }

        //método q vai criar os produtos
        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000), //cria um preço aleatório até 1000
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user
            }); ;
        }
    }
}
