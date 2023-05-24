using System.ComponentModel.DataAnnotations;
using System;

namespace SuperShop.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Data annotation
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] // n usa C2 no modo edição -> C currency
        public decimal Price { get; set; }

        [Display(Name="Image")] //como vai aparecer o campo imagem no site
        public string ImageUrl { get; set; }

        [Display(Name = "LastPurchase")]
        public DateTime LastPurchase { get; set; }

        [Display(Name = "LastSale")]
        public DateTime LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)] //N -> number
        public double Stock { get; set; }
    }
}
