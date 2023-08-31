using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

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

      _countriesService = new CountriesService(null!);
    }

    #region AddCountry
    [Fact]
    public async Task AddCountry_NullCountry()
    {
      //Arrange
      CountryAddRequest? request = null;

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCountry_NullCountryName()
    {
      //Arrange
      CountryAddRequest request = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, null as string)
        .Create();

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
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

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(request1);
        await _countriesService.AddCountry(request2);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
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
      response.CountryID.Should().NotBe(Guid.Empty);
      countries_from_GetAllCountries.Should().Contain(response);

    }
    #endregion

    #region GetAllCountries
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
      //Act
      List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

      //Assert
      actual_country_response_list.Should().BeEmpty();
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
      actual_country_response.Should().BeEquivalentTo(countries_list_from_add_country);
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
      country_response_from_get_method.Should().BeNull();
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
      country_response_from_get.Should().Be(country_response_from_add);
    }
    #endregion
  }
}
