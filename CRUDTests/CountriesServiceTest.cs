using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;

namespace CRUDTests
{
  public class CountriesServiceTest
  {
    private readonly ICountriesService _countriesService;
    private readonly ICountriesRespository _countriesRespository;
    private readonly Mock<ICountriesRespository> _countriesRespositoryMock;
    private readonly IFixture _fixture;

    public CountriesServiceTest()
    {
      _fixture = new Fixture();
      _countriesRespositoryMock = new Mock<ICountriesRespository>();
      _countriesRespository = _countriesRespositoryMock.Object;
      _countriesService = new CountriesService(_countriesRespository);
    }

    #region AddCountry
    [Fact]
    public async Task AddCountry_NullCountry_ToBeArgumentNullException()
    {
      //Arrange
      CountryAddRequest? country_add_request = null;

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(country_add_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCountry_NullCountryName_ToBeArgumentException()
    {
      //Arrange
      CountryAddRequest country_add_request = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, null as string)
        .Create();

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(country_add_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
    {
      //Arrange
      CountryAddRequest country_add_request_1 = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, "USA")
        .Create();
      CountryAddRequest country_add_request_2 = _fixture.Build<CountryAddRequest>()
        .With(temp => temp.CountryName, "USA")
        .Create();

      Country country_1 = country_add_request_1.ToCountry();
      Country country_2 = country_add_request_2.ToCountry();

      _countriesRespositoryMock.SetupSequence(temp => temp.AddCountry(It.IsAny<Country>()))
        .ReturnsAsync(country_1)
        .ReturnsAsync(country_2);

      _countriesRespositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
        .ReturnsAsync(country_2);

      //Act
      Func<Task> action = async () =>
      {
        await _countriesService.AddCountry(country_add_request_1);
        await _countriesService.AddCountry(country_add_request_2);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountry_FullCountryDetails_ToBeSuccessful()
    {
      //Arrange
      CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();

      CountryResponse country_response_expected = country_add_request.ToCountry().ToCountryResponse();

      //Act
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);
      country_response_expected.CountryID = country_response_from_add.CountryID;

      //Assert
      country_response_from_add.CountryID.Should().NotBe(Guid.Empty);
      country_response_from_add.Should().Be(country_response_expected);

    }
    #endregion

    #region GetAllCountries
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
      //Arrange
      _countriesRespositoryMock.Setup(temp => temp.GetAllCountries())
        .ReturnsAsync(new List<Country>());

      //Act
      List<CountryResponse> countries_list_from_get = await _countriesService.GetAllCountries();

      //Assert
      countries_list_from_get.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
    {
      //Arrange
      List<Country> countries = new()
      {
        _fixture.Create<Country>(),
        _fixture.Create<Country>(),
      };

      List<CountryResponse> countries_list_expected = countries.Select(temp => temp.ToCountryResponse()).ToList();

      _countriesRespositoryMock.Setup(temp => temp.GetAllCountries())
        .ReturnsAsync(countries);

      //Act
      List<CountryResponse> countries_list_from_get = await _countriesService.GetAllCountries();

      //Assert
      countries_list_from_get.Should().BeEquivalentTo(countries_list_expected);
    }
    #endregion

    #region GetCountryByCountryID
    [Fact]
    public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
    {
      //Arrange
      Guid? countryID = null;

      //Act
      CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(countryID);

      //Assert
      country_response_from_get.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
    {
      //Arrange
      Country country = _fixture.Create<Country>();
      CountryResponse country_response_expected = country.ToCountryResponse();

      _countriesRespositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
        .ReturnsAsync(country);

      //Act
      CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country.CountryID);

      //Assert
      country_response_from_get.Should().Be(country_response_expected);
    }
    #endregion
  }
}
