using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    public interface IImageHelper
    {
        //método para o upload
        //esta string vai ser o caminho para a BD
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
