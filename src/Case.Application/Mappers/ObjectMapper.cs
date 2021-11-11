using AutoMapper;
using Case.Application.Models;
using Case.Core.Entities;
using System;

namespace Case.Application.Mappers
{
    // The best implementation of AutoMapper for class libraries - https://stackoverflow.com/questions/26458731/how-to-configure-auto-mapper-in-class-library-project
    public class ObjectMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<ApplicationDtoMapper>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });
        public static IMapper Mapper => Lazy.Value;

        public class ApplicationDtoMapper : Profile
        {
            public ApplicationDtoMapper()
            {
                CreateMap<Post, PostDto>().ReverseMap();
            }
        }
    }
}
