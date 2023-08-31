using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
  public class CountriesService : ICountriesService
  {
    private readonly ICountriesRespository _countriesRepository;

    public CountriesService(ICountriesRespository countriesRepository)
    {
      _countriesRepository = countriesRepository;
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

      if(await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
      {
        throw new ArgumentException("Given country name already exists");
      }

      Country country = countryAddRequest.ToCountry();

      country.CountryID = Guid.NewGuid();

      await _countriesRepository.AddCountry(country);

      return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
      return (await _countriesRepository.GetAllCountries()).Select(temp => temp.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
      if (countryID == null) return null;

      Country? country_response_from_list = await _countriesRepository.GetCountryByCountryID(countryID) ?? null;

      return country_response_from_list?.ToCountryResponse();
    }
  }
}