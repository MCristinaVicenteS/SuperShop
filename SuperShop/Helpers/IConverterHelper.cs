using SuperShop.Data.Entities;
using SuperShop.Models;

namespace SuperShop.Helpers
{
    public interface IConverterHelper
    {
        //converter o viewmodel em product
        //o bool server para qd editar - para saber se o id é inserido aqui ou se vem pela tabela
        //no path passa o caminho para a imagem
        Product ToProduct(ProductViewModel modle, string path, bool isNew);

        //converter o product em viewModel
        ProductViewModel ToProductViewModel(Product product);
    }
}
