using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
      _countriesService = new CountriesService(false);
    }

    #region AddCountry
    [Fact]
    public void AddCountry_NullCountry()
    {
      //Arrange
      CountryAddRequest? request = null;

      //Assert
      Assert.Throws<ArgumentNullException>(() =>
      {
        //Act
        _countriesService.AddCountry(request);
      });
    }

    [Fact]
    public void AddCountry_NullCountryName()
    {
      //Arrange
      CountryAddRequest request = new() { CountryName = null };

      //Assert
      Assert.Throws<ArgumentException>(() =>
      {
        //Act
        _countriesService.AddCountry(request);
      });
    }

    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
      //Arrange
      CountryAddRequest request1 = new() { CountryName = "USA" };
      CountryAddRequest request2 = new() { CountryName = "USA" };

      //Assert
      Assert.Throws<ArgumentException>(() =>
      {
        //Act
        _countriesService.AddCountry(request1);
        _countriesService.AddCountry(request2);
      });
    }

    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
      //Arrange
      CountryAddRequest request = new() { CountryName = "Japan" };

      //Act
      CountryResponse response = _countriesService.AddCountry(request);
      List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

      //Assert
      Assert.True(response.CountryID != Guid.Empty);
      //internally calls Equals method that we have overriden
      Assert.Contains(response, countries_from_GetAllCountries);

    }
    #endregion

    #region GetAllCountries
    [Fact]
    public void GetAllCountries_EmptyList()
    {
      //Act
      List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

      //Assert
      Assert.Empty(actual_country_response_list);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
      //Arrange
      List<CountryAddRequest> country_request_list = new()
      {
        new CountryAddRequest() {  CountryName = "USA" },
        new CountryAddRequest() {  CountryName = "UK" },
      };

      //Act
      List<CountryResponse> countries_list_from_add_country = new();
      foreach (CountryAddRequest country_request in country_request_list)
      {
        CountryResponse response = _countriesService.AddCountry(country_request);
        countries_list_from_add_country.Add(response);
      }
      List<CountryResponse> actual_country_response = _countriesService.GetAllCountries();

      //Assert
      //Assert.Equal(countries_list_from_add_country, actual_country_response);
      foreach (CountryResponse expected_country in countries_list_from_add_country)
      {
        Assert.Contains(expected_country, actual_country_response);
      }
    }
    #endregion

    #region GetCountryByCountryID
    [Fact]
    public void GetCountryByCountryID_NullCountryID()
    {
      //Arrange
      Guid? countryID = null;

      //Act
      CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);

      //Assert
      Assert.Null(country_response_from_get_method);
    }

    [Fact]
    public void GetCountryByCountryID_ValidCountryID()
    {
      //Arrange
      CountryAddRequest country_add_request = new() { CountryName = "China" };
      CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

      //Act
      CountryResponse? country_response_from_get = _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

      //Assert
      //Assert.NotNull(country_response_from_get);
      Assert.Equal(country_response_from_add, country_response_from_get);
    }
    #endregion
  }
}
