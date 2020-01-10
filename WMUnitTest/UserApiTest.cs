using System;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using WorkManagement;

namespace WMUnitTest
{
    public class UserApiTest
    {
        private readonly HttpClient _client;
        public UserApiTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }
        [Theory]
        [InlineData("GET",1)]
        public async Task GetAllUser(string method, int? adminId = null)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/users/GetAllUser/{adminId}");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
