using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace TiendaVirtual.Ayudadores
{
    public class BlobAyudas : IBlobAyudas
    {
        private readonly CloudBlobClient _blobClient;

        public BlobAyudas(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }
        public async Task DeleteBlobAsync(Guid id, string contenedorNombre)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(contenedorNombre);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{id}");
            await blockBlob.DeleteAsync();

        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string contenedorNombre)
        {

            Stream stream = file.OpenReadStream();
            return await UploadBlobAsync(stream, contenedorNombre);


        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string contenedorNombre)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadBlobAsync(stream, contenedorNombre);
        }

        public async Task<Guid> UploadBlobAsync(string image, string contenedorNombre)
        {
            Stream stream = File.OpenRead(image);
            return await UploadBlobAsync(stream, contenedorNombre);
        }

        private async Task<Guid> UploadBlobAsync(Stream stream, string contenedorNombre)
        {
            Guid name = Guid.NewGuid();
            CloudBlobContainer container = _blobClient.GetContainerReference(contenedorNombre);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }


    }
    }
