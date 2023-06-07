using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;

        //injectar o IRepository
        public ProductsController(IProductRepository productRepository, IUserHelper userHelper) 
        {
            _productRepository = productRepository; //n é preciso instanciar o objecto pq uso o injector de dependências -> startup.cs           
            _userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p => p.Name)); //vai ao repository e trás todos os produtos e ordena por nome
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value); //com .value aceita id sem valor e não rebenta
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create() //nao tem nenhum parÂmetro -> n envia produtos. MAS qd clicar no botão submit (view) -> vai enviar dados para fora
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                //carregar a imagem
                var path = string.Empty;

                if(model.ImageFile != null && model.ImageFile.Length > 0) //se o model tiver uma imagem
                {
                    //criar uma chave aleatório para impedir nomes de imagens iguais
                    var guid = Guid.NewGuid().ToString();
                    var file =$"{guid}.jpg";

                    //caminho od vou gravar
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file);

                    //gravar no servidor
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    //gravar o caminho relativo na base de dados no campo URL (n grava até ao servidor)
                    path = $"~/images/products/{file}";
                }

                //converter o ProductViewModel em Product -> pq quero continuar a gravar o Product na BD
                var product = this.ToProduct(model, path);

                //TODO: modificar para o user que tiver logado
                product.User = await _userHelper.GetUserByEmailAsync("rafaasfs@gmail.com");
                await _productRepository.CreateAsync(product); //se for válido -> fica guardado em memória -> n na BD
                return RedirectToAction(nameof(Index)); //qd estiver gravado -> redirecciona para a action index -> mostra a lista dos produtos
            }
            return View(model); //se n passar na validação -> deixa os dados nos campos mas n os grava
        }

        private Product ToProduct(ProductViewModel model, string path) //vai ter de retornar um product
        {
            return new Product
            {
                Id = model.Id,
                ImageUrl = path,
                IsAvailable = model.IsAvailable,
                LastPurchase = model.LastPurchase,
                LastSale = model.LastSale,
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
                User = model.User,
            };
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) //tem ? para n forçar o user a colocar um id no url -> é opcional; Sem o ?, se n colocasse id -> rebentava
        {
            if (id == null)
            {
                return NotFound();
            }

            //dupla segurança
            var product = await _productRepository.GetByIdAsync(id.Value); //vai ver na memória se o id colocado no url -> existe ou n
            if (product == null)
            {
                return NotFound();
            }

            //Converter o product em productViewModel para aparecer a imagem
            var model= this.ToProductViewModel(product);

            return View(model); //se encotrar o produto -> mostra-o
        }

        private ProductViewModel ToProductViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                IsAvailable = product.IsAvailable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model) //productviewmodel
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl; //n é empty para o caso de n alterar a imagem

                    if(model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var file = $"{guid}.jpg";

                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/images/products/{file}";
                    }

                    var product = this.ToProduct(model, path);

                    //TODO: modificar para o user que tiver logado
                    product.User = await _userHelper.GetUserByEmailAsync("rafaasfs@gmail.com");
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await _productRepository.ExistAsync(model.Id)) //se o produto n existir (por ex,pq alg apagou) -> retorna
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); //no final redirecciona para o index
            }
            return View(model); //se alguma coisa correr mal -> retorna com o produto como estava
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")] //isto é preciso para saber que vai receber informaçao http do tipo POST -> para executar esta action
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await _productRepository.DeleteAsync(product);            
            return RedirectToAction(nameof(Index));
        }
    }
}
