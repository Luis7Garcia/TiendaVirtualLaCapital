namespace TiendaVirtual.Ayudadores
{
    public interface IBlobAyudas
    {

        Task<Guid> UploadBlobAsync(IFormFile file, string contenedorNombre);

        Task<Guid> UploadBlobAsync(byte[] file, string contenedorNombre);

        Task<Guid> UploadBlobAsync(string image, string contenedorNombre);

        Task DeleteBlobAsync(Guid id, string contenedorNombre);
    }
}
