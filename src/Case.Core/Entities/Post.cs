using Case.Core.Entities.Base;
using System.Collections.Generic;

namespace Case.Core.Entities
{
    public class Post : MongoDbEntity
    {
        public Post()
        {
            SliderIds = new HashSet<string>();
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public ICollection<string> SliderIds { get; set; }
    }
}
