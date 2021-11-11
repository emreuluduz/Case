using Case.Application.Interfaces;
using Case.Application.Models;
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
    [Route("v{version:apiVersion}/sliders")]
    public class SlidersController : ControllerBase
    {
        private readonly ILogger<SlidersController> _logger;
        private readonly ISliderService _sliderService;
        private readonly IPostService _postService;
        private readonly ICacheService _cacheService;

        public SlidersController(ILogger<SlidersController> logger, ISliderService sliderService, IPostService postService, ICacheService cacheService)
        {
            _logger = logger;
            _sliderService = sliderService;
            _postService = postService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get slider posts by slug
        /// </summary>
        /// <param name="slug">Slug of Slider</param>
        /// <remarks>Returns slider posts</remarks>
        [HttpGet("{slug}/posts")]
        public async Task<IActionResult> GetSliderPostsAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                _logger.LogWarning("Slug boş olamaz");
                return BadRequest();
            }
            var posts = _cacheService.Get<IEnumerable<Post>>(CacheNamespaces.SliderPosts(slug));
            if (posts == null || !posts.Any())
            {
                var slider = await _sliderService.GetAsync(x => x.Slug.Equals(slug));
                posts = slider.Posts;
                _cacheService.Set(CacheNamespaces.SliderPosts(slug), posts, TimeSpan.FromMinutes(1));
            }
            return Ok(posts);
        }

        /// <summary>
        /// Find slider by ID
        /// </summary>
        /// <param name="id">ID of Slider</param>
        /// <remarks>Returns a single slider</remarks>
        [HttpGet("{id}", Name = "GetSliderById")]
        [ProducesResponseType(200, Type = typeof(Slider))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id boş olamaz");
                return BadRequest();
            }

            return Ok(await _sliderService.GetByIdAsync(id));
        }

        /// <summary>
        /// Add a new slider
        /// </summary>
        /// <param name="sliderDto">Slider object that needs to be added</param>
        /// <remarks></remarks>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> PostAsync([FromBody] CreateSliderDto sliderDto)
        {
            if (sliderDto == null)
            {
                _logger.LogWarning("Slider modeli boş olamaz");
                return BadRequest();
            }
            else if (await _sliderService.GetAsync(x => x.Slug.Equals(sliderDto.Slug)) != null)
            {
                return BadRequest("There is already a slider exist with same slug!");
            }

            sliderDto.Posts = sliderDto.Posts.OrderBy(x => x.Order);
            var postIds = sliderDto.Posts.Select(p => p.PostId);
            var posts = _postService.Get(x => postIds.Contains(x.Id)).ToList();
            var orderedPosts = new List<Post>();
            foreach (var item in sliderDto.Posts)
            {
                orderedPosts.Add(posts.First(x => x.Id.Equals(item.PostId)));
            }

            var slider = new Slider
            {
                Name = sliderDto.Name,
                Slug = sliderDto.Slug,
                Posts = orderedPosts
            };

            var added = await _sliderService.AddAsync(slider);
            foreach (var item in posts)
            {
                if (item.SliderIds == null)
                {
                    item.SliderIds = new List<string>();
                }
                item.SliderIds.Add(added.Id);
                await _postService.UpdateAsync(item.Id, item);
            }
            return CreatedAtRoute("GetSliderById", new { version = "1", id = added.Id }, added);
        }

        /// <summary>
        /// Update a slider
        /// </summary>
        /// <param name="id">Slider ID that needs to be updated</param>
        /// <param name="sliderDto">Slider object that needs to be updated</param>
        /// <remarks></remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(OkResult))]
        [ProducesResponseType(400, Type = typeof(BadRequestResult))]
        [ProducesResponseType(500, Type = typeof(Exception))]
        public async Task<IActionResult> PutAsync(string id, [FromBody] CreateSliderDto sliderDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Id boş olamaz");
                return BadRequest();
            }
            else if (sliderDto != null)
            {
                _logger.LogWarning("Slider model boş olamaz");
                return BadRequest();
            }

            sliderDto.Posts = sliderDto.Posts.OrderBy(x => x.Order);
            var postIds = sliderDto.Posts.Select(p => p.PostId);
            var posts = _postService.Get(x => postIds.Contains(x.Id)).ToList();
            var orderedPosts = new List<Post>();
            foreach (var item in sliderDto.Posts)
            {
                orderedPosts.Add(posts.First(x => x.Id.Equals(item.PostId)));
            }

            var updated = await _sliderService.GetByIdAsync(id);
            var removed = updated.Posts.Where(x => !postIds.Contains(x.Id));

            updated.Name = sliderDto.Name;
            updated.Slug = sliderDto.Slug;
            updated.Posts = orderedPosts;
            if (removed.Any())
            {
                foreach (var item in removed)
                {
                    item.SliderIds.Remove(id);
                    await _postService.UpdateAsync(item.Id, item);
                }
            }
            await _sliderService.UpdateAsync(id, updated);
            _logger.LogInformation("{0} sliderı güncellendi", sliderDto.Slug);
            foreach (var item in posts)
            {
                if (!item.SliderIds.Contains(id))
                {
                    item.SliderIds.Add(id);
                    await _postService.UpdateAsync(item.Id, item);
                }
            }
            await _cacheService.RemoveAsync(CacheNamespaces.SliderPosts(sliderDto.Slug));
            return Ok();
        }

        /// <summary>
        /// Delete a slider
        /// </summary>
        /// <param name="id">Slider ID that needs to be deleted</param>
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

            var slider = await _sliderService.GetByIdAsync(id);
            if (slider != null)
            {
                await _sliderService.DeleteAsync(id);
                await _cacheService.RemoveAsync(CacheNamespaces.SliderPosts(slider.Slug));

                _logger.LogInformation("{0} sliderı silindi", slider.Slug);
            }
            return Ok();
        }
    }
}
