using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;
    private readonly IPersonsRepository _personsRepository;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      _fixture = new Fixture();
      _personsRepositoryMock = new Mock<IPersonsRepository>();
      _personsRepository = _personsRepositoryMock.Object;
      _personsService = new PersonsService(_personsRepository);
      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
      //Arrange
      PersonAddRequest? person_add_request = null;

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.AddPerson(person_add_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
      //Arrange
      PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.PersonName, null as string)
        .Create();

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.AddPerson(person_add_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
      //Arrange
      PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
        .With(temp => temp.Email, "someone@example.com")
        .Create();

      PersonResponse person_response_expected = person_add_request.ToPerson().ToPersonResponse();

      //Act
      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
      person_response_expected.PersonID = person_response_from_add.PersonID;

      //Assert
      person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
      person_response_from_add.Should().Be(person_response_expected);
    }
    #endregion

    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
      //Arrange
      _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
        .ReturnsAsync(new List<Person>());

      //Act
      List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

      //Assert
      persons_list_from_get.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons_ToBeSuccessful()
    {
      //Arrange
      Person person_1 = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone1@example.com")
        .Create();
      Person person_2 = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone2@example.com")
        .Create();

      List<Person> persons = new() { person_1, person_2 };

      List<PersonResponse> persons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_expected in persons_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_expected.ToString());
      }

      _personsRepositoryMock.Setup(temp => temp.GetAllPersons())
        .ReturnsAsync(persons);

      //Act
      List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_get in persons_list_from_get)
      {
        _testOutputHelper.WriteLine(person_response_from_get.ToString());
      }

      //Assert
      persons_list_from_get.Should().BeEquivalentTo(persons_list_expected);
    }
    #endregion

    #region GetPersonByPersonID
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
      //Arrange
      Guid? personID = null;

      //Act
      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

      //Assert
      person_response_from_get.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByPersonID_ValidPersonID_ToBeSuccessful()
    {
      //Arrange
      Person person = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone@example.com")
        .Create();
      PersonResponse person_response_expected = person.ToPersonResponse();

      _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
        .ReturnsAsync(person);

      //Act
      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person.PersonID);

      //Assert
      person_response_from_get.Should().Be(person_response_expected);
    }
    #endregion

    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersons_ToBeSuccessful()
    {
      //Arrange
      Person person_1 = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone1@example.com")
        .Create();
      Person person_2 = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone2@example.com")
        .Create();

      List<Person> persons = new() { person_1, person_2 };

      List<PersonResponse> persons_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_expected in persons_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_expected.ToString());
      }

      _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
        .ReturnsAsync(persons);

      //Act
      List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonAddRequest.PersonName), "");

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_search in persons_list_from_search)
      {
        _testOutputHelper.WriteLine(person_response_from_search.ToString());
      }

      //Assert
      persons_list_from_search.Should().BeEquivalentTo(persons_list_expected);
    }
    #endregion

    #region GetSortedPerson
    [Fact]
    public void GetSortedPerson_ToBeSuccessful()
    {
      //Arrange
      Person person_1 = _fixture.Build<Person>()
       .With(temp => temp.PersonName, "Marko")
       .With(temp => temp.Email, "someone1@example.com")
       .Create();
      Person person_2 = _fixture.Build<Person>()
        .With(temp => temp.PersonName, "Mario")
        .With(temp => temp.Email, "someone2@example.com")
        .Create();
      Person person_3 = _fixture.Build<Person>()
        .With(temp => temp.PersonName, "Ana")
        .With(temp => temp.Email, "someone3@example.com")
        .Create();

      List<Person> persons = new() { person_1, person_2, person_3 };

      List<PersonResponse> persons_list_expected = persons.Select(temp => temp.ToPersonResponse()).OrderByDescending(temp => temp.PersonName).ToList();

      _testOutputHelper.WriteLine("Expected:");
      foreach (PersonResponse person_response_expected in persons_list_expected)
      {
        _testOutputHelper.WriteLine(person_response_expected.ToString());
      }

      //Act
      List<PersonResponse> persons_list_from_sort = _personsService.GetSortedPersons(persons.Select(temp => temp.ToPersonResponse()).ToList(), nameof(Person.PersonName), SortOrderOptions.DESC);

      _testOutputHelper.WriteLine("Actual:");
      foreach (PersonResponse person_response_from_sort in persons_list_from_sort)
      {
        _testOutputHelper.WriteLine(person_response_from_sort.ToString());
      }

      //Assert
      persons_list_from_sort.Should().ContainInOrder(persons_list_expected);
    }
    #endregion

    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
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
    public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
    {
      //Arrange
      Person person = _fixture.Build<Person>()
       .With(temp => temp.PersonName, null as string)
       .With(temp => temp.Email, "someone@example.com")
       .With(temp => temp.Gender, "Other")
       .Create();

      PersonUpdateRequest person_update_request = person.ToPersonResponse().ToPersonUpdateRequest();

      //Act
      Func<Task> action = async () =>
      {
        await _personsService.UpdatePerson(person_update_request);
      };

      //Assert
      await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
    {
      //Arrange
      Person person = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone@example.com")
        .With(temp => temp.Gender, "Other")
        .Create();

      PersonResponse person_response_expected = person.ToPersonResponse();

      PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

      _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
       .ReturnsAsync(person);

      //Act
      PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

      //Assert
      person_response_from_update.Should().Be(person_response_expected);
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
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
      //Arrange
      Person person = _fixture.Build<Person>()
        .With(temp => temp.Email, "someone@example.com")
        .Create();

      _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>()))
       .ReturnsAsync(true);

      //Act
      bool isDeleted = await _personsService.DeletePerson(person.PersonID);

      //Assert
      isDeleted.Should().BeTrue();
    }
    #endregion
  }
}
