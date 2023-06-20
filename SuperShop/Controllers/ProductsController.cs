using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        //injectar o IRepository
        public ProductsController(IProductRepository productRepository, IUserHelper userHelper, IBlobHelper blobHelper, IConverterHelper converterHelper) 
        {
            _productRepository = productRepository; //n é preciso instanciar o objecto pq uso o injector de dependências -> startup.cs           
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
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
        [Authorize]
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
                Guid imageId = Guid.Empty;

                if(model.ImageFile != null && model.ImageFile.Length > 0) //se o model tiver uma imagem
                {
                    //usar esse método -> enviar o ficheiro e guardar nessa pasta
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                }

                //converter o ProductViewModel em Product -> pq quero continuar a gravar o Product na BD
                var product = _converterHelper.ToProduct(model, imageId, true);


                //associar o produto criado ao user que tiver logado
                product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _productRepository.CreateAsync(product); //se for válido -> fica guardado em memória -> n na BD
                return RedirectToAction(nameof(Index)); //qd estiver gravado -> redirecciona para a action index -> mostra a lista dos produtos
            }
            return View(model); //se n passar na validação -> deixa os dados nos campos mas n os grava
        }

        //private Product ToProduct(ProductViewModel model, string path) //vai ter de retornar um product
        //{
        //    return new Product
        //    {
        //        Id = model.Id,
        //        ImageUrl = path,
        //        IsAvailable = model.IsAvailable,
        //        LastPurchase = model.LastPurchase,
        //        LastSale = model.LastSale,
        //        Name = model.Name,
        //        Price = model.Price,
        //        Stock = model.Stock,
        //        User = model.User,
        //    };
        //}

        // GET: Products/Edit/5
        [Authorize]
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
            var model= _converterHelper.ToProductViewModel(product);

            return View(model); //se encotrar o produto -> mostra-o
        }

        //private ProductViewModel ToProductViewModel(Product product)
        //{
        //    return new ProductViewModel
        //    {
        //        Id = product.Id,
        //        IsAvailable = product.IsAvailable,
        //        LastPurchase = product.LastPurchase,
        //        LastSale = product.LastSale,
        //        ImageUrl = product.ImageUrl,
        //        Name = product.Name,
        //        Price = product.Price,
        //        Stock = product.Stock,
        //        User = product.User
        //    };
        //}

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
                    Guid imageId = model.ImageId; //n é empty para o caso de n alterar a imagem

                    if(model.ImageFile != null && model.ImageFile.Length > 0)
                    {                        
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                    }

                    var product = _converterHelper.ToProduct(model, imageId, false);

                    //Associar o produto criado ao user que tiver logado
                    product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
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
        [Authorize]
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
