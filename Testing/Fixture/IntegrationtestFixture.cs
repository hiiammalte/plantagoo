using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plantagoo.Api;
using Plantagoo.Data;
using Plantagoo.Encryption;
using Plantagoo.Entities;
using System;

namespace Plantagoo.Testing.Fixture
{
    public class IntegrationtestFixture : WebApplicationFactory<Startup>, IDisposable
    {
        protected ServiceProvider _provider;
        private bool _disposed = false;

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder
                .UseEnvironment("Testing")
                .UseConfiguration(
                    new ConfigurationBuilder()
                    .AddUserSecrets<Startup>()
                    .Build()
                )
                .ConfigureServices(services =>
                {
                    _provider = services.BuildServiceProvider();
                    using var scope = _provider.CreateScope();
                    var scopedProvider = scope.ServiceProvider;
                    var db = scopedProvider.GetRequiredService<AppDbContext>();
                    var hasher = scopedProvider.GetRequiredService<IPasswordHasher>();
                    try
                    {
                        db.Database.Migrate();
                        db.Users.Add(new UserModel
                        {
                            Email = "johndoe@integration.test",
                            Password = hasher.Hash("integrationtestPW"),
                            FirstName = "Jane",
                            LastName = "Doe"
                        });
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred creating database. Error: {ex.Message}");
                    }
                });

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                using var scope = _provider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                try
                {
                    db.Database.EnsureDeleted();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred deleting database. Error: {ex.Message}");
                }
            }

            base.Dispose(disposing);
        }
    }
}
