using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data;

namespace SuperShop.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        //receber os produtos todos do repository -> com GET
        [HttpGet]
        public ActionResult GetProducts()
        {
            return Ok(_productRepository.GetAllWithUsers()); //dados em json através da função Ok
        }
    }
}
