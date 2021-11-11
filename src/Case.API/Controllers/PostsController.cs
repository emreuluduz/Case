using Case.Application.Interfaces;
using Case.Core.Entities;
using Case.Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Case.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/posts")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostService _postService;
        private readonly ISliderService _sliderService;
        private readonly ICacheService _cacheService;

        public PostsController(ILogger<PostsController> logger, IPostService postService, ISliderService sliderService, ICacheService cacheService)
        {
            _logger = logger;
            _postService = postService;
            _sliderService = sliderService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get posts
        /// </summary>
        /// <remarks>Returns posts list</remarks>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Post>))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public IActionResult GetAsync()
        {
            var posts = _cacheService.Get<IEnumerable<Post>>(CacheNamespaces.Posts);

            if (posts == null || !posts.Any())
            {
                posts = _postService.Get().ToList();
                _cacheService.Set(CacheNamespaces.Posts, posts, TimeSpan.FromMinutes(1));
            }
            _logger.LogTrace("{0} haber getirildi", posts.Count());

            return Ok(posts);
        }

        /// <summary>
        /// Find post by ID
        /// </summary>
        /// <param name="id">ID of Post</param>
        /// <remarks>Returns a single post</remarks>
        [HttpGet("{id}", Name = "GetPostById")]
        [ProducesResponseType(200, Type = typeof(Post))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id boş olamaz");
                return BadRequest();
            }
            _logger.LogTrace("{0} ID'li haber getirilliyor", id);

            return Ok(await _postService.GetByIdAsync(id));
        }

        /// <summary>
        /// Add a new post
        /// </summary>
        /// <param name="post">Post object that needs to be added</param>
        /// <remarks></remarks>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> PostAsync([FromBody] Post post)
        {
            if (post == null)
            {
                _logger.LogWarning("Post model boş olamaz");
                return BadRequest();
            }

            var added = await _postService.AddAsync(post);
            _logger.LogInformation("'{0}' başlıklı haber eklendi güncellendi", post.Title);
            return CreatedAtRoute("GetPostById", new { version = "1", id = added.Id }, added);
        }

        /// <summary>
        /// Bulk add new posts
        /// </summary>
        /// <param name="posts">Post objects that needs to be added</param>
        /// <remarks></remarks>
        [HttpPost("bulk")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> PostAsync([FromBody] IEnumerable<Post> posts)
        {
            if (!posts.Any())
            {
                _logger.LogWarning("Post listesi boş olamaz");
                return BadRequest();
            }

            var status = await _postService.AddRangeAsync(posts);
            _logger.LogInformation("{0} adet haber eklendi", posts.Count());
            return Ok(status);
        }

        /// <summary>
        /// Update a post
        /// </summary>
        /// <param name="id">Post ID that needs to be updated</param>
        /// <param name="post">Post object that needs to be updated</param>
        /// <remarks></remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> PutAsync(string id, [FromBody] Post post)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id boş olamaz");
                return BadRequest();
            }
            else if (post != null)
            {
                _logger.LogWarning("Post model boş olamaz");
                return BadRequest();
            }
            var updated = await _postService.GetByIdAsync(id);
            updated.Title = post.Title;
            updated.Description = post.Description;
            updated.Link = post.Link;
            updated.UpdatedAt = DateTime.UtcNow;
            await _postService.UpdateAsync(id, updated);
            _logger.LogInformation("{0} idli haber güncellendi", post.Id);
            if (updated.SliderIds.Any())
            {
                var sliders = _sliderService.Get(x => updated.SliderIds.Contains(x.Id));
                foreach (var slider in sliders)
                {
                    var sliderPost = slider.Posts.FirstOrDefault(x => x.Id == id);
                    if (sliderPost != null)
                    {

                        sliderPost.Title = updated.Title;
                        sliderPost.Description = updated.Description;
                        sliderPost.Link = updated.Link;
                        sliderPost.UpdatedAt = updated.UpdatedAt;
                        await _sliderService.UpdateAsync(slider.Id, slider);
                        await _cacheService.RemoveAsync(CacheNamespaces.SliderPosts(slider.Slug));
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="id">Post ID that needs to be deleted</param>
        /// <remarks></remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id boş olamaz");
                return BadRequest();
            }
            await _postService.DeleteAsync(id);
            _logger.LogInformation("{0} haber silindi", id);
            return Ok();
        }
    }
}
