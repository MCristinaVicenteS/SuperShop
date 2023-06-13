using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    public interface IBlobHelper
    {
        //método q vai devolver uma guid 
        //recebe o ficheio e o parâmetro com o nome do contentor que queremos usar
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        //array de bytes - para o mobile
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);

        //através de uma string
        Task<Guid> UploadBlobAsync(string image, string containerName);
    }
}
