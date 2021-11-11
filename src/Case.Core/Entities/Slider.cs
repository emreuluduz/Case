using Case.Core.Entities.Base;
using System.Collections.Generic;

namespace Case.Core.Entities
{
    public class Slider : MongoDbEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
