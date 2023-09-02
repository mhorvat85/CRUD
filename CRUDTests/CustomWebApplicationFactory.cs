﻿using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests
{
  public class CustomWebApplicationFactory : WebApplicationFactory<Program>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      base.ConfigureWebHost(builder);

      builder.UseEnvironment("Testing");

      builder.ConfigureServices(services =>
      {
        var descriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

        if (descriptor != null)
        {
          services.Remove(descriptor);
        }
        services.AddDbContext<ApplicationDbContext>(options =>
        {
          options.UseInMemoryDatabase("DbForTesting");
        });
      });
    }
  }
}
