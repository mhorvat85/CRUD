using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Reflection;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly PersonsDbContext _db;

    public PersonsService(PersonsDbContext personsDbContext)
    {
      _db = personsDbContext;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
      if (personAddRequest == null)
      {
        throw new ArgumentNullException(nameof(personAddRequest));
      }

      ValidationHelper.ModelValidation(personAddRequest);

      Person person = personAddRequest.ToPerson();

      person.PersonID = Guid.NewGuid();

      _db.Persons.Add(person);
      await _db.SaveChangesAsync();

      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
      return await _db.Persons.Include("Country").Select(person => person.ToPersonResponse()).ToListAsync();
    }

    public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
      if (personID == null) return null;

      Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
      if (person == null) return null; 
      
      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<PersonResponse> allPersons = await GetAllPersons();
      List<PersonResponse> matchingPersons = allPersons;

      if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) return matchingPersons;

      switch (searchBy)
      {
        case nameof(PersonResponse.PersonName):
          matchingPersons = allPersons.Where(temp => string.IsNullOrEmpty(temp.PersonName) || temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        case nameof(PersonResponse.Email):
          matchingPersons = allPersons.Where(temp => string.IsNullOrEmpty(temp.Email) || temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        case nameof(PersonResponse.DateOfBirth):
          matchingPersons = allPersons.Where(temp => (temp.DateOfBirth == null) || temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        case nameof(PersonResponse.Gender):
          matchingPersons = allPersons.Where(temp => string.IsNullOrEmpty(temp.Gender) || temp.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        case nameof(PersonResponse.CountryID):
          matchingPersons = allPersons.Where(temp => string.IsNullOrEmpty(temp.Country) || temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        case nameof(PersonResponse.Address):
          matchingPersons = allPersons.Where(temp => string.IsNullOrEmpty(temp.Address) || temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
        default: matchingPersons = allPersons; break;
      }
      return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
      if (string.IsNullOrEmpty(sortBy)) return allPersons;

      List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
      {
        (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.DateOfBirth).ToList(),
        (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),
        (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.CountryID), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.CountryID), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),
        (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),
        (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Age).ToList(),
        (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Age).ToList(),
        _ => allPersons
      };
      return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
      if (personUpdateRequest == null)
      {
        throw new ArgumentNullException(nameof(personUpdateRequest));
      }

      ValidationHelper.ModelValidation(personUpdateRequest);

      Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID) ?? throw new ArgumentException("Given person id doesn't exist");

      foreach (PropertyInfo prop in matchingPerson.GetType().GetProperties())
      {
        if (prop.Name != nameof(Person.PersonID))
        {
          string propertyName = prop.Name;
          var otherPropValue = personUpdateRequest.GetType().GetProperty(propertyName)?.GetValue(personUpdateRequest);

          if (propertyName == nameof(Person.Gender)) otherPropValue = otherPropValue?.ToString();
          
          matchingPerson.GetType().GetProperty(propertyName)?.SetValue(matchingPerson, otherPropValue);
        }
      }
      await _db.SaveChangesAsync();

      return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
      if (personID == null)
      {
        throw new ArgumentNullException(nameof(personID));
      }

      Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
      if (person == null) return false;

      _db.Persons.Remove(person);
      await _db.SaveChangesAsync();

      return true;
    }
  }
}
