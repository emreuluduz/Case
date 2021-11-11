using Case.Application.Models;
using Case.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Case.API.Tests
{
    public class SlidersControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient _client;

        public SlidersControllerTests(WebApplicationFactory<Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task Create_Slider_Should_Return_Id()
        {
            var rnd = new Random();
            var response = await _client.GetAsync("/v1/posts");
            var posts = JsonConvert.DeserializeObject<Post[]>(await response.Content.ReadAsStringAsync());
            PostOrder[] postOrders = new PostOrder[10];
            for (int i = 0; i < 10; i++)
            {
                var order = new PostOrder { PostId = posts[rnd.Next(1, 60)].Id, Order = i + 1 };
                postOrders[i] = order;
            }
            var number = rnd.Next(1, 1000);
            var slider = new CreateSliderDto
            {
                Name = "Manþet" + number,
                Slug = "manset" + number,
                Posts = postOrders
            };

            var content = new StringContent(JsonConvert.SerializeObject(slider), Encoding.UTF8, "application/json");
            var sliderResponse = await _client.PostAsync("/v1/sliders", content);
            sliderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Get_Slider_With_Slug_Should_Count_Ten()
        {
            string slug = "manset353";
            var response = await _client.GetAsync($"/v1/sliders/{slug}/posts");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var posts = JsonConvert.DeserializeObject<Post[]>(await response.Content.ReadAsStringAsync());
            posts.Should().HaveCount(10);
        }
    }
}
