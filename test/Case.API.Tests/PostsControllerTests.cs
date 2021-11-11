using Case.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Case.API.Tests
{
    public class PostsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        readonly HttpClient _client;

        public PostsControllerTests(WebApplicationFactory<Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task Get_Posts_Should_Be_OK_And_NotNull()
        {
            var response = await _client.GetAsync("/v1/posts");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var posts = JsonConvert.DeserializeObject<Post[]>(await response.Content.ReadAsStringAsync());
            posts.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_Post_Should_Created()
        {
            var post = new Post
            {
                Title = "Son Dakika: 24 Temmuz Altýn fiyatlarý düþüþte! Bugün Çeyrek altýn, gram altýn fiyatlarý canlý 2021 - Habertürk",
                Description = "Altýn fiyatlarý düþüþte... 24 Temmuz Cumartesi günü altýn fiyatlarý yatýrýmcýlar tarafýndan merakla takip ediliyor. Güncel piyasalarda altýn fiyatlarý düþüþe geçerken gram altýnýn fiyatý 499,49 liradan, çeyrek altýn ise 810,09 liradan iþlem görüyor. Peki, gra…",
                Link = "https://www.haberturk.com/son-dakika-24-temmuz-altin-fiyatlari-dususte-bugun-ceyrek-altin-gram-altin-fiyatlari-canli-2021-3142296-ekonomi"
            };
            var content = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/v1/posts", content);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Bulk_Create_Post_Should_Return_True()
        {
            var posts = new Post[] {
                new Post
                {
                    Title = "Son Dakika: 24 Temmuz Altýn fiyatlarý düþüþte! Bugün Çeyrek altýn, gram altýn fiyatlarý canlý 2021 - Habertürk",
                    Description = "Altýn fiyatlarý düþüþte... 24 Temmuz Cumartesi günü altýn fiyatlarý yatýrýmcýlar tarafýndan merakla takip ediliyor. Güncel piyasalarda altýn fiyatlarý düþüþe geçerken gram altýnýn fiyatý 499,49 liradan, çeyrek altýn ise 810,09 liradan iþlem görüyor. Peki, gra…",
                    Link = "https://www.haberturk.com/son-dakika-24-temmuz-altin-fiyatlari-dususte-bugun-ceyrek-altin-gram-altin-fiyatlari-canli-2021-3142296-ekonomi"
                },
                new Post
                {
                    Title = "Coinbase Bu 3 Yeni Altcoin Ýçin Destek Ekliyor – Sýrada Bu 16 Kripto Para Olabilir! - KoinFinans",
                    Description = "En iyi kripto para borsasý olan Coinbase, bu üç yeni varlýk için destek ekledi. Platformun izleme listesindeki 16 altcoin de deðerlendirme aþamasýnda ve",
                    Link = "https://www.koinfinans.com/coinbase-bu-3-yeni-altcoin-icin-destek-ekliyor-sirada-bu-16-altcoin-olabilir/"
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(posts), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/v1/posts", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var status = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            status.Should().BeTrue();
        }
    }
}
