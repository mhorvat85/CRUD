using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
  public class CountriesService : ICountriesService
  {
    private readonly PersonsDbContext _db;

    public CountriesService(PersonsDbContext personsDbContext)
    {
      _db = personsDbContext;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
      if (countryAddRequest == null)
      {
        throw new ArgumentNullException(nameof(countryAddRequest));
      }

      if (countryAddRequest.CountryName == null)
      {
        throw new ArgumentException(nameof(countryAddRequest.CountryName));
      }

      if(await _db.Countries.AnyAsync(country => country.CountryName == countryAddRequest.CountryName))
      {
        throw new ArgumentException("Given country name already exists");
      }

      Country country = countryAddRequest.ToCountry();

      country.CountryID = Guid.NewGuid();

      _db.Countries.Add(country);
      await _db.SaveChangesAsync();

      return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
      return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
      if (countryID == null) return null;

      Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(temp =>  temp.CountryID == countryID);

      return country_response_from_list?.ToCountryResponse();
    }
  }
}