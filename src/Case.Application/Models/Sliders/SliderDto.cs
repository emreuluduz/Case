using System.Collections.Generic;

namespace Case.Application.Models
{
    public class SliderDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public ICollection<PostDto> Posts { get; set; }
    }

    public class CreateSliderDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public IEnumerable<PostOrder> Posts { get; set; }
    }

    public class PostOrder
    {
        public string PostId { get; set; }
        public int Order { get; set; }
    }
}
