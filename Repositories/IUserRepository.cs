using Microsoft.AspNetCore.Identity;

namespace FORMATION.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAll();
    }
}
