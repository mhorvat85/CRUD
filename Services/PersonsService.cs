using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Reflection;

namespace Services
{
  public class PersonsService : IPersonsService
  {
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;

    public PersonsService(bool initialize = true)
    {
      _persons = new List<Person>();
      _countriesService = new CountriesService();


      if (initialize)
      {
        _persons.Add(new Person() { PersonID = Guid.Parse("CBC79104-6A21-4A19-9E7D-61DF93AAD480"), PersonName = "Ashil", Email = "acardow0@google.cn", DateOfBirth = DateTime.Parse("2000-01-03"), Gender = "Female", Address = "40 Farmco Road", ReceiveNewsLetters = false, CountryID = Guid.Parse("2F8BF0C1-B9AD-42F6-82B2-0608EEE3CB64") });

        _persons.Add(new Person() { PersonID = Guid.Parse("07F6D99C-BEE4-4A05-A70B-3308D66F3342"), PersonName = "Bettina", Email = "bcisec1@time.com", DateOfBirth = DateTime.Parse("1991-07-20"), Gender = "Female", Address = "1696 Rigney Terrace", ReceiveNewsLetters = true, CountryID = Guid.Parse("8809E927-DA93-473F-8F59-4BA81CEC32ED") });

        _persons.Add(new Person() { PersonID = Guid.Parse("3F01D8A1-7CD0-4851-89C5-0F6BC2CAA7DD"), PersonName = "Julee", Email = "jbardey2@sbwire.com", DateOfBirth = DateTime.Parse("1993-05-01"), Gender = "Female", Address = "4514 School Street", ReceiveNewsLetters = false, CountryID = Guid.Parse("31CA84BA-3A97-493F-A37C-D8B660AA6506") });

        _persons.Add(new Person() { PersonID = Guid.Parse("21AD5290-7A1E-4834-8F16-15A9A7797D35"), PersonName = "Evelin", Email = "elettice3@senate.gov", DateOfBirth = DateTime.Parse("1990-09-21"), Gender = "Male", Address = "31 Harper Lane", ReceiveNewsLetters = false, CountryID = Guid.Parse("45420716-F7B8-485D-9D44-74D132116C8A") });

        _persons.Add(new Person() { PersonID = Guid.Parse("BA3C80CA-25B3-4BAF-98CE-E9506EBA5809"), PersonName = "Dyna", Email = "dgetty4@jugem.jp", DateOfBirth = DateTime.Parse("1997-06-26"), Gender = "Female", Address = "54 Kenwood Crossing", ReceiveNewsLetters = false, CountryID = Guid.Parse("E743B204-B1C3-4110-9991-EAB9F1DEB730") });

        _persons.Add(new Person() { PersonID = Guid.Parse("DD76228C-0BC0-488F-B26E-3FDE6CA15118"), PersonName = "Eugenia", Email = "eanthona5@phoca.cz", DateOfBirth = DateTime.Parse("1990-03-04"), Gender = "Female", Address = "58 Cordelia Court", ReceiveNewsLetters = true, CountryID = Guid.Parse("2F8BF0C1-B9AD-42F6-82B2-0608EEE3CB64") });

        _persons.Add(new Person() { PersonID = Guid.Parse("14A40D93-BFE3-45BA-9D74-A66CA8EC46C8"), PersonName = "Arlen", Email = "amontfort6@princeton.edu", DateOfBirth = DateTime.Parse("1991-06-12"), Gender = "Female", Address = "6 Nancy Trail", ReceiveNewsLetters = false, CountryID = Guid.Parse("8809E927-DA93-473F-8F59-4BA81CEC32ED") });

        _persons.Add(new Person() { PersonID = Guid.Parse("B6D2D078-6669-43D8-8CF7-6410BE755332"), PersonName = "Rubi", Email = "rcastagneto7@twitter.com", DateOfBirth = DateTime.Parse("1998-07-30"), Gender = "Female", Address = "73 Westend Hill", ReceiveNewsLetters = false, CountryID = Guid.Parse("31CA84BA-3A97-493F-A37C-D8B660AA6506") });

        _persons.Add(new Person() { PersonID = Guid.Parse("72F99B33-F519-4844-8C34-E106C965B9BA"), PersonName = "Christin", Email = "cstuehmeyer8@businesswire.com", DateOfBirth = DateTime.Parse("1996-06-14"), Gender = "Female", Address = "76000 Rowland Parkway", ReceiveNewsLetters = false, CountryID = Guid.Parse("45420716-F7B8-485D-9D44-74D132116C8A") });

        _persons.Add(new Person() { PersonID = Guid.Parse("B50BC0B9-C638-40BE-8CC4-60E0E9BA24BC"), PersonName = "Maury", Email = "mlandells9@technorati.com", DateOfBirth = DateTime.Parse("1995-01-27"), Gender = "Male", Address = "648 Eagan Center", ReceiveNewsLetters = true, CountryID = Guid.Parse("E743B204-B1C3-4110-9991-EAB9F1DEB730") });
      }
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
      PersonResponse personResponse = person.ToPersonResponse();
      personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;

      return personResponse;
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
      if (personAddRequest == null)
      {
        throw new ArgumentNullException(nameof(personAddRequest));
      }

      ValidationHelper.ModelValidation(personAddRequest);

      Person person = personAddRequest.ToPerson();

      person.PersonID = Guid.NewGuid();

      _persons.Add(person);

      return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
      return _persons.Select(ConvertPersonToPersonResponse).ToList();
    }

    public PersonResponse? GetPersonByPersonID(Guid? personID)
    {
      if (personID == null) return null;

      Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
      if (person == null) return null; 
      
      return person.ToPersonResponse();
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
      List<PersonResponse> allPersons = GetAllPersons();
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

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
      if (personUpdateRequest == null)
      {
        throw new ArgumentNullException(nameof(personUpdateRequest));
      }

      ValidationHelper.ModelValidation(personUpdateRequest);

      Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID) ?? throw new ArgumentException("Given person id doesn't exist");

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
      return matchingPerson.ToPersonResponse();
    }

    public bool DeletePerson(Guid? personID)
    {
      if (personID == null)
      {
        throw new ArgumentNullException(nameof(personID));
      }

      Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
      if (person == null) return false;

      _persons.Remove(person);
      return true;
    }
  }
}
