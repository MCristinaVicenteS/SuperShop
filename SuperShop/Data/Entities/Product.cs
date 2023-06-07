using System.ComponentModel.DataAnnotations;
using System;

namespace SuperShop.Data.Entities
{
    public class Product : IEntity
    {
        public int Id { get; set; }

        //data annotation para ser obrigatório preencher o nome e com limite máximo
        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can contain {1} length")] //mensagem de erro -> o campo 0 é o campo name; o 1 é o parâmetro 50 (length). NOTA: n vai aparecer
        public string Name { get; set; }

        //Data annotation
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] // n usa C2 no modo edição -> C currency
        public decimal Price { get; set; }

        [Display(Name="Image")] //como vai aparecer o campo imagem no site
        public string ImageUrl { get; set; }

        [Display(Name = "LastPurchase")]
        public DateTime? LastPurchase { get; set; } //? -> opcional

        [Display(Name = "LastSale")]
        public DateTime? LastSale { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)] //N -> number
        public double Stock { get; set; }

        //o user que criou o produto
        public User User { get; set; }

        public string ImageFullPath
        {
            get
            {
                if(string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }
                return $"https://localhost:44315{ImageUrl.Substring(1)}";
            }
        }
    }
}
