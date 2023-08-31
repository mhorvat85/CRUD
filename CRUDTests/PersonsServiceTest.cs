using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      _fixture = new Fixture();

      List<Country> countriesInitialData = new() { };
      List<Person> personsInitialData = new() { };

      DbContextMock<ApplicationDbContext> dbContextMock = new(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
      dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
      dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

      ApplicationDbContext dbContext = dbContextMock.Object;

      _countriesService = new CountriesService(dbContext);
      _personsService = new PersonsService(dbContext);

      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public async Task AddPerson_NullPerson()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.AddPerson(personAddRequest);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddPerson_NullPersonName()
    {
      //Arrange
      PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, null as string)
        .Create();

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.AddPerson(personAddRequest);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
      //Arrange
      PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .Create();

      //Act
      PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
      List<PersonResponse> persons_list = await _personsService.GetAllPersons();

      //Assert
      person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
      persons_list.Should().Contain(person_response_from_add);
    }
    #endregion

    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
      //Act
      List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

      //Assert
      persons_from_get.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
      //Arrange
      CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
      CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone1@example.com")
        .With(temp => temp.CountryID, country_response_1.CountryID)
        .Create();
      PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone2@example.com")
        .With(temp => temp.CountryID, country_response_2.CountryID)
        .Create();

      List<PersonAddRequest> persons_request = new() { person_request_1, person_request_2 };
      List<PersonResponse> persons_list_from_add = new();

      foreach (PersonAddRequest person in persons_request)
      {
        PersonResponse person_response = await _personsService.AddPerson(person);
        persons_list_from_add.Add(person_response);
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      //Act
      List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_get)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      persons_list_from_get.Should().BeEquivalentTo(persons_list_from_add);
    }
    #endregion

    #region GetPersonByPersonID
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
      //Arrange
      Guid? personID = null;

      //Act
      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

      //Assert
      person_response_from_get.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByPersonID_ValidPersonID()
    {
      //Arrange
      CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
        
      CountryResponse country_response = await _countriesService.AddCountry(country_request);

      //Act
      PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .With(temp => temp.CountryID, country_response.CountryID)
        .Create();

      PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);

      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

      //Assert
      person_response_from_get.Should().Be(person_response_from_add);
    }
    #endregion

    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
      //Arrange
      CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
      CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone1@example.com")
        .With(temp => temp.CountryID, country_response_1.CountryID)
        .Create();
      PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone2@example.com")
        .With(temp => temp.CountryID, country_response_1.CountryID)
        .Create();

      List<PersonAddRequest> persons_request = new() { person_request_1, person_request_2 };
      List<PersonResponse> persons_list_from_add = new();

      foreach (PersonAddRequest person in persons_request)
      {
        PersonResponse person_response = await _personsService.AddPerson(person);
        persons_list_from_add.Add(person_response);
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      //Act
      List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonAddRequest.PersonName), "");

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      persons_list_from_search.Should().BeEquivalentTo(persons_list_from_add);
    }

    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
      //Arrange
      CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
      CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
       .With(temp => temp.PersonName, "Marko")
       .With(temp => temp.Email, "someone1@example.com")
       .With(temp => temp.CountryID, country_response_1.CountryID)
       .Create();
      PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, "Mario")
        .With(temp => temp.Email, "someone2@example.com")
        .With(temp => temp.CountryID, country_response_2.CountryID)
        .Create();
      PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, "Ana")
        .With(temp => temp.Email, "someone3@example.com")
        .With(temp => temp.CountryID, country_response_1.CountryID)
        .Create();

      List<PersonAddRequest> persons_request = new() { person_request_1, person_request_2, person_request_3 };
      List<PersonResponse> persons_list_from_add = new();

      foreach (PersonAddRequest person in persons_request)
      {
        PersonResponse person_response = await _personsService.AddPerson(person);
        persons_list_from_add.Add(person_response);
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      //Act
      List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonAddRequest.PersonName), "ma");

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      persons_list_from_search.Should().OnlyContain(temp => temp.PersonName!.Contains("ma", StringComparison.OrdinalIgnoreCase));
    }
    #endregion

    #region GetSortedPerson
    [Fact]
    public async Task GetSortedPerson()
    {
      //Arrange
      CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
      CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
       .With(temp => temp.PersonName, "Marko")
       .With(temp => temp.Email, "someone1@example.com")
       .With(temp => temp.CountryID, country_response_1.CountryID)
       .Create();
      PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, "Mario")
        .With(temp => temp.Email, "someone2@example.com")
        .With(temp => temp.CountryID, country_response_2.CountryID)
        .Create();
      PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, "Ana")
        .With(temp => temp.Email, "someone3@example.com")
        .With(temp => temp.CountryID, country_response_1.CountryID)
        .Create();

      List<PersonAddRequest> persons_request = new() { person_request_1, person_request_2, person_request_3 };
      List<PersonResponse> persons_list_from_add = new();

      foreach (PersonAddRequest person in persons_request)
      {
        PersonResponse person_response = await _personsService.AddPerson(person);
        persons_list_from_add.Add(person_response);
      }

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        _testOutputHelper.WriteLine(person_response_from_add.ToString());
      }

      List<PersonResponse> allPersons = await _personsService.GetAllPersons();
      //Act
      List<PersonResponse> persons_list_from_sort = _personsService.GetSortedPersons(allPersons, nameof(PersonAddRequest.PersonName), SortOrderOptions.DESC);

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_sort)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
    }
    #endregion

    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = null;

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(person_update_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
      //Arrange
      PersonUpdateRequest person_update_request = _fixture.Create<PersonUpdateRequest>();

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(person_update_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
      //Arrange
      CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .With(temp => temp.CountryID, country_response_from_add.CountryID)
        .Create();
      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
      person_update_request.PersonName = null;

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(person_update_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_PersonFullDetails()
    {
      //Arrange
      CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .With(temp => temp.CountryID, country_response_from_add.CountryID)
        .Create();
      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
      person_update_request.PersonName = "Janko";
      person_update_request.Email = "janko@example.com";

      //Act
      PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);
      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

      //Assert
      person_response_from_get.Should().Be(person_response_from_update);
    }
    #endregion

    #region DeletePerson
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
      //Act
      bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

      //Assert
      isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
      //Arrange
      CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .With(temp => temp.CountryID, country_response_from_add.CountryID)
        .Create();

      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      //Act
      bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

      //Assert
      isDeleted.Should().BeTrue();
    }
    #endregion
  }
}
