using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUD.StartupExtensions
{
  public static class ConfigureServicesExtension
  {
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddControllersWithViews();

      services.AddScoped<ICountriesService, CountriesService>();
      services.AddScoped<IPersonsService, PersonsService>();
      services.AddScoped<ICountriesRespository, CountriesRepository>();
      services.AddScoped<IPersonsRepository, PersonsRepository>();

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
      });

      return services;
    }
  }
}
