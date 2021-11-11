using Case.Core.Entities;
using Case.Core.Interfaces.Repositories.Base;

namespace Case.Application.Interfaces
{
    public interface IPostService : IRepository<Post, string>
    {
    }
}
