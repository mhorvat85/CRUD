﻿using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
  public class CountriesService : ICountriesService
  {
    private readonly List<Country> _countries;

    public CountriesService()
    {
      _countries = new List<Country>();
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