using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
  public class CountriesService : ICountriesService
  {
    private readonly List<Country> _countries;

    public CountriesService(bool initialize = true)
    {
      _countries = new List<Country>();
      if (initialize)
      {
        _countries.AddRange(new List<Country>()
        {
          new Country() { CountryID = Guid.Parse("2F8BF0C1-B9AD-42F6-82B2-0608EEE3CB64"), CountryName = "USA" },
          new Country() { CountryID = Guid.Parse("8809E927-DA93-473F-8F59-4BA81CEC32ED"), CountryName = "UK" },
          new Country() { CountryID = Guid.Parse("31CA84BA-3A97-493F-A37C-D8B660AA6506"), CountryName = "France" },
          new Country() { CountryID = Guid.Parse("45420716-F7B8-485D-9D44-74D132116C8A"), CountryName = "Japan" },
          new Country() { CountryID = Guid.Parse("E743B204-B1C3-4110-9991-EAB9F1DEB730"), CountryName = "Australia" },
        });
      }
    }

    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
      if (countryAddRequest == null)
      {
        throw new ArgumentNullException(nameof(countryAddRequest));
      }

      if (countryAddRequest.CountryName == null)
      {
        throw new ArgumentException(nameof(countryAddRequest.CountryName));
      }

      if(_countries.Any(country => country.CountryName == countryAddRequest.CountryName))
      {
        throw new ArgumentException("Given country name already exists");
      }

      Country country = countryAddRequest.ToCountry();

      country.CountryID = Guid.NewGuid();

      _countries.Add(country);

      return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
      return _countries.Select(country =>  country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryID(Guid? countryID)
    {
      if (countryID == null) return null;

      Country? country_response_from_list = _countries.FirstOrDefault(temp =>  temp.CountryID == countryID);

      return country_response_from_list?.ToCountryResponse();
    }
  }
}