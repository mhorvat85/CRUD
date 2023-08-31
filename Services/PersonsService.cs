using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly IPersonsRepository _personsRepository;

    public PersonsService(IPersonsRepository personsRepository)
    {
      _personsRepository = personsRepository;
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

      await _personsRepository.AddPerson(person);

      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
      return (await _personsRepository.GetAllPersons()).Select(temp => temp.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
      if (personID == null) return null;

      Person? person = await _personsRepository.GetPersonByPersonID(personID);
      if (person == null) return null; 
      
      return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<Person> persons = searchBy switch
      {
        nameof(PersonResponse.PersonName) =>
          await _personsRepository.GetFilteredPersons(person => person.PersonName!.Contains(searchString!)),
        nameof(PersonResponse.Email) =>
          await _personsRepository.GetFilteredPersons(person => person.Email!.Contains(searchString!)),
        nameof(PersonResponse.DateOfBirth) =>
          await _personsRepository.GetFilteredPersons(person => person.DateOfBirth!.Value.ToString("dd MMMM yyyy").Contains(searchString!)),
        nameof(PersonResponse.Gender) =>
          await _personsRepository.GetFilteredPersons(person => person.Gender!.Equals(searchString!)),
        nameof(PersonResponse.CountryID) =>
          await _personsRepository.GetFilteredPersons(person => person.Country!.CountryName!.Contains(searchString!)),
        nameof(PersonResponse.Address) =>
          await _personsRepository.GetFilteredPersons(person => person.Address!.Contains(searchString!)),
        _ => await _personsRepository.GetAllPersons()
      };
      return persons.Select(temp => temp.ToPersonResponse()).ToList();
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

      return (await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson())).ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
      if (personID == null)
      {
        throw new ArgumentNullException(nameof(personID));
      }

      return await _personsRepository.DeletePerson(personID);
    }
  }
}
