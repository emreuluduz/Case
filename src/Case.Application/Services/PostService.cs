using Case.Application.Interfaces;
using Case.Core.Entities;
using Case.Infrastructure;
using Case.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace Case.Application.Services
{
    public class PostService : Repository<Post>, IPostService
    {
        public PostService(IOptions<MongoDbSettings> options) : base(options)
        {

        }
    }
}
