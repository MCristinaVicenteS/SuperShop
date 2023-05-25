using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync()); //vai ao datacontex -> à propriedade dos produtos -> e trás todos em lista para dentro da view
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Create([Bind("Id,Name,Price,ImageUrl,LastPurchase,LastSale,IsAvailable,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product); //se for válido -> fica guardado em memória -> n na BD
                await _context.SaveChangesAsync(); //grava na BD
                return RedirectToAction(nameof(Index)); //qd estiver gravado -> redirecciona para a action index -> mostra a lista dos produtos
            }
            return View(product); //se n passar na validação -> deixa os dados nos campos mas n os grava
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) //tem ? para n forçar o user a colocar um id no url -> é opcional; Sem o ?, se n colocasse id -> rebentava
        {
            if (id == null)
            {
                return NotFound();
            }

            //dupla segurança
            var product = await _context.Products.FindAsync(id); //vai ver na memória se o id colocado no url -> existe ou n
            if (product == null)
            {
                return NotFound();
            }
            return View(product); //se encotrar o produto -> mostra-o
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product) //recebe o id e o produto completo
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) //se o id já n existir pq alg apagou -> retorna
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
            return View(product); //se alguma coisa correr mal -> retorna com o produto como estava
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id) //método auxiliar para procurar o produto
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
