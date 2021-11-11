using Case.Application.Interfaces;
using Case.Core.Entities;
using Case.Infrastructure;
using Case.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

namespace Case.Application.Services
{
    public class SliderService : Repository<Slider>, ISliderService
    {
        public SliderService(IOptions<MongoDbSettings> options) : base(options)
        {

        }
    }
}
