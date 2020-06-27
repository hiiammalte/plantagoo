using Newtonsoft.Json;
using Plantagoo.DTOs.Users;
using Plantagoo.Testing.Base;
using Plantagoo.Testing.Fixture;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Plantagoo.Testing
{
    public class UsersControllerTests : IntegrationtestBase
    {
        public UsersControllerTests(IntegrationtestFixture fixture) : base(fixture)
        { }

        [Fact]
        public async Task RegisterUser_WithValidCredentials_Returns201()
        {
            //Arrange
            var user = new UserRegisterDTO
            {
                Email = "johndoe@test.com",
                Password = "skeletonPassword",
                FirstName = "John",
                LastName = "Doe"
            };

            //Act
            var response = await _client.PostAsync("api/v1/users", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task FindAll_WithOnlyOneUserInDb_ReturnsSuccessListsOne()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await _client.GetAsync("api/v1/users");
            response.EnsureSuccessStatusCode();
            var users = JsonConvert.DeserializeObject<List<UserDetailsDTO>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.Single(users);
        }
    }
}
