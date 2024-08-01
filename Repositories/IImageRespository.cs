namespace FORMATION.Repositories
{
    public interface IImageRespository
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
