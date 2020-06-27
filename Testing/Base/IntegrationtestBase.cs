using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plantagoo.DTOs.Authentication;
using Plantagoo.Testing.Fixture;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Plantagoo.Testing.Base
{
    public class IntegrationtestBase : IClassFixture<IntegrationtestFixture>
    {
        protected readonly IntegrationtestFixture _fixture;
        protected readonly HttpClient _client;

        public IntegrationtestBase(IntegrationtestFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        protected async Task AuthenticateAsync()
        {
            var credentials = new CredentialsDTO
            {
                Email = "johndoe@integration.test",
                Password = "integrationtestPW"
            };

            var response = await _client.PostAsync("api/v1/authentication", new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            dynamic data = JObject.Parse(await response.Content.ReadAsStringAsync());

            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(string.Concat("Bearer ", data.jwt));
        }
    }
}
