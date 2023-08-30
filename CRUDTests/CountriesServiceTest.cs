using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;

namespace CRUDTests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
      var countriesInitialData = new List<Country>() { };

      DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
      dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

      ApplicationDbContext dbContext = dbContextMock.Object;

      _countriesService = new CountriesService(dbContext);
    }

    #region AddCountry
    [Fact]
    public async Task AddCountry_NullCountry()
    {
      //Arrange
      CountryAddRequest? request = null;

      //Assert
      await Assert.ThrowsAsync<ArgumentNullException>(async() =>
      {
        //Act
        await _countriesService.AddCountry(request);
      });
    }

    [Fact]
    public async Task AddCountry_NullCountryName()
    {
      //Arrange
      CountryAddRequest request = new() { CountryName = null };

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async() =>
      {
        //Act
        await _countriesService.AddCountry(request);
      });
    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
      //Arrange
      CountryAddRequest request1 = new() { CountryName = "USA" };
      CountryAddRequest request2 = new() { CountryName = "USA" };

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async() =>
      {
        //Act
        await _countriesService.AddCountry(request1);
        await _countriesService.AddCountry(request2);
      });
    }

    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
      //Arrange
      CountryAddRequest request = new() { CountryName = "Japan" };

      //Act
      CountryResponse response = await _countriesService.AddCountry(request);
      List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

      //Assert
      Assert.True(response.CountryID != Guid.Empty);
      //internally calls Equals method that we have overriden
      Assert.Contains(response, countries_from_GetAllCountries);

    }
    #endregion

    #region GetAllCountries
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
      //Act
      List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

      //Assert
      Assert.Empty(actual_country_response_list);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
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
        CountryResponse response = await _countriesService.AddCountry(country_request);
        countries_list_from_add_country.Add(response);
      }
      List<CountryResponse> actual_country_response = await _countriesService.GetAllCountries();

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
    public async Task GetCountryByCountryID_NullCountryID()
    {
      //Arrange
      Guid? countryID = null;

      //Act
      CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

      //Assert
      Assert.Null(country_response_from_get_method);
    }

    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID()
    {
      //Arrange
      CountryAddRequest country_add_request = new() { CountryName = "China" };
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      //Act
      CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

      //Assert
      //Assert.NotNull(country_response_from_get);
      Assert.Equal(country_response_from_add, country_response_from_get);
    }
    #endregion
  }
}
