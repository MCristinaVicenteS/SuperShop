using Microsoft.AspNetCore.Mvc.Rendering;
using SuperShop.Data.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SuperShop.Data
{
    //inserir no Startup
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable GetAllWithUsers();

        //Prepara a lista para inserir os produtos na combobox do Html ->AddItemViewModel
        IEnumerable<SelectListItem> GetComboProducts();
    }
}
