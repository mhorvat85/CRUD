using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;

    public PersonsServiceTest()
    {
      _personsService = new PersonsService();
    }

    #region AddPerson
    [Fact]
    public void AddPerson_NullPerson()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;

      //Assert
      Assert.Throws<ArgumentNullException>(() =>
      {
        //Act
        _personsService.AddPerson(personAddRequest);
      });
    }

    [Fact]
    public void AddPerson_NullPersonName()
    {
      //Arrange
      PersonAddRequest? personAddRequest = new() { PersonName = null };

      //Assert
      Assert.Throws<ArgumentException>(() =>
      {
        //Act
        _personsService.AddPerson(personAddRequest);
      });
    }

    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
      //Arrange
      PersonAddRequest? personAddRequest = new() { PersonName = "Marko", Email = "marko@example.com", Address = "somewhere 1, nowhere", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };

      //Act
      PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
      List<PersonResponse> persons_list = _personsService.GetAllPersons();

      //Assert
      Assert.True(person_response_from_add.PersonID != Guid.Empty);
      Assert.Contains(person_response_from_add, persons_list);
    }
    #endregion

  }
}
