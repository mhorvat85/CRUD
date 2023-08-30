using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CRUDTests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    public CountriesServiceTest()
    {
      _fixture = new Fixture();

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
      CountryAddRequest request = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, null as string)
        .Create();

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
      CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, "USA")
        .Create();
      CountryAddRequest request2 = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, "USA")
        .Create();

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
      CountryAddRequest request = _fixture.Create<CountryAddRequest>();

      //Act
      CountryResponse response = await _countriesService.AddCountry(request);
      List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

      //Assert
      Assert.True(response.CountryID != Guid.Empty);
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
        _fixture.Create<CountryAddRequest>(),
        _fixture.Create < CountryAddRequest>(),
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
      CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      //Act
      CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

      //Assert
      Assert.Equal(country_response_from_add, country_response_from_get);
    }
    #endregion
  }
}
