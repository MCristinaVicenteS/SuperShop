﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;
using SuperShop.Models;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(this.User.Identity.Name);
            return View(model);
        }

        //Aparece a view da lista temporária do user logado
        public async Task<IActionResult> Create()
        {
            var model = await _orderRepository.GetDetailTempAsync(this.User.Identity.Name);
            return View(model);
        }

        //vai buscar a combobox ao ProductRepository e mostra a view AddProduct
        public IActionResult AddProduct()
        {
            var model = new AddItemViewModel
            {
                Quantity = 1,
                Products = _productRepository.GetComboProducts()
            };

            return View(model);        
        }
    }
}
