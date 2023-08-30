﻿using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
  public class PersonsServiceTest
  {
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
      _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
      _personsService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
      _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public async Task AddPerson_NullPerson()
    {
      //Arrange
      PersonAddRequest? personAddRequest = null;

      //Assert
      await Assert.ThrowsAsync<ArgumentNullException>(async () =>
      {
        //Act
        await _personsService.AddPerson(personAddRequest);
      });
    }

    [Fact]
    public async Task AddPerson_NullPersonName()
    {
      //Arrange
      PersonAddRequest? personAddRequest = new() { PersonName = null };

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async () =>
      {
        //Act
        await _personsService.AddPerson(personAddRequest);
      });
    }

    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
      //Arrange
      PersonAddRequest? personAddRequest = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };

      //Act
      PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
      List<PersonResponse> persons_list = await _personsService.GetAllPersons();

      //Assert
      Assert.True(person_response_from_add.PersonID != Guid.Empty);
      Assert.Contains(person_response_from_add, persons_list);
    }
    #endregion

    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
      //Act
      List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

      //Assert
      Assert.Empty(persons_from_get);
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
      //Arrange
      CountryAddRequest country_request_1 = new() { CountryName = "USA" };
      CountryAddRequest country_request_2 = new() { CountryName = "UK" };

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
      PersonAddRequest person_request_2 = new() { PersonName = "Mario", Email = "mario@example.com", Address = "SomeStreet 2, SomeTown", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = true };

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
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        Assert.Contains(person_response_from_add, persons_list_from_get);
      }
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
      Assert.Null(person_response_from_get);
    }

    [Fact]
    public async Task GetPersonByPersonID_ValidPersonID()
    {
      //Arrange
      CountryAddRequest country_request = new() { CountryName = "Canada" };
      CountryResponse country_response = await _countriesService.AddCountry(country_request);

      //Act
      PersonAddRequest person_request = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

      PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);

      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

      //Assert
      Assert.Equal(person_response_from_add, person_response_from_get);
    }
    #endregion

    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
      //Arrange
      CountryAddRequest country_request_1 = new() { CountryName = "USA" };
      CountryAddRequest country_request_2 = new() { CountryName = "UK" };

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
      PersonAddRequest person_request_2 = new() { PersonName = "Mario", Email = "mario@example.com", Address = "SomeStreet 2, SomeTown", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = true };

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
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        Assert.Contains(person_response_from_add, persons_list_from_search);
      }
    }

    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
      //Arrange
      CountryAddRequest country_request_1 = new() { CountryName = "USA" };
      CountryAddRequest country_request_2 = new() { CountryName = "UK" };

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
      PersonAddRequest person_request_2 = new() { PersonName = "Mario", Email = "mario@example.com", Address = "SomeStreet 2, SomeTown", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = true };
      PersonAddRequest person_request_3 = new() { PersonName = "Ana", Email = "ana@example.com", Address = "SomeStreet 3, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Female, ReceiveNewsLetters = true };

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
      foreach (PersonResponse person_response_from_add in persons_list_from_add)
      {
        if (person_response_from_add.PersonName != null)
        {
          if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
          {
            Assert.Contains(person_response_from_add, persons_list_from_search);
          }
        }
      }
    }
    #endregion

    #region GetSortedPerson
    [Fact]
    public async Task GetSortedPerson()
    {
      //Arrange
      CountryAddRequest country_request_1 = new() { CountryName = "USA" };
      CountryAddRequest country_request_2 = new() { CountryName = "UK" };

      CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
      CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

      PersonAddRequest person_request_1 = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };
      PersonAddRequest person_request_2 = new() { PersonName = "Mario", Email = "mario@example.com", Address = "SomeStreet 2, SomeTown", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = true };
      PersonAddRequest person_request_3 = new() { PersonName = "Ana", Email = "ana@example.com", Address = "SomeStreet 3, SomeTown", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Female, ReceiveNewsLetters = true };

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
      persons_list_from_add = persons_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

      //Assert
      for (int i = 0; i < persons_list_from_add.Count; i++)
      {
        Assert.Equal(persons_list_from_add[i], persons_list_from_sort[i]);
      }
    }
    #endregion

    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = null;

      //Assert
      await Assert.ThrowsAsync<ArgumentNullException>(async () =>
      {
        //Act
        await _personsService.UpdatePerson(person_update_request);
      });
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
      //Arrange
      PersonUpdateRequest? person_update_request = new() { PersonID = Guid.NewGuid() };

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async () =>
      {
        //Act
        await _personsService.UpdatePerson(person_update_request);
      });
    }

    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
      //Arrange
      CountryAddRequest country_add_request = new() { CountryName = "UK" };
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = new() { PersonName = "Marko", Email = "marko@example.com", Gender = GenderOptions.Male, CountryID = country_response_from_add.CountryID };
      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
      person_update_request.PersonName = null;

      //Assert
      await Assert.ThrowsAsync<ArgumentException>(async () =>
      {
        //Act
        await _personsService.UpdatePerson(person_update_request);
      });
    }

    [Fact]
    public async Task UpdatePerson_PersonFullDetails()
    {
      //Arrange
      CountryAddRequest country_add_request = new() { CountryName = "UK" };
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_from_add.CountryID, Gender = GenderOptions.Male, ReceiveNewsLetters = false };
      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
      person_update_request.PersonName = "Janko";
      person_update_request.Email = "janko@example.com";

      //Act
      PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);
      PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_update.PersonID);

      //Assert
      Assert.Equal(person_response_from_update, person_response_from_get);
    }
    #endregion

    #region DeletePerson
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
      //Act
      bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

      //Assert
      Assert.False(isDeleted);
    }

    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
      //Arrange
      CountryAddRequest country_add_request = new() { CountryName = "USA" };
      CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

      PersonAddRequest person_add_request = new() { PersonName = "Marko", Email = "marko@example.com", Address = "SomeStreet 1, SomeTown", CountryID = country_response_from_add.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

      PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

      //Act
      bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

      //Assert
      Assert.True(isDeleted);
    }
    #endregion
  }
}
