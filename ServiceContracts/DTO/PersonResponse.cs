using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
  public class PersonResponse
  {
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
      if (obj == null) return false;
      if (obj.GetType() != typeof(PersonResponse)) return false;

      PersonResponse person = (PersonResponse)obj;
      return PersonID == person.PersonID && PersonName == person.PersonName && Email == person.Email && DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryID == person.CountryID && Address == person.Address && ReceiveNewsLetters == person.ReceiveNewsLetters && Age == person.Age;
    }
    
    public override string ToString()
    {
      return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date Of Birth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}, Country ID: {CountryID}, Country: {Country}, Address: {Address}, Receive NewsLetters: {ReceiveNewsLetters}, Age: {Age}";
    }

    public override int GetHashCode()
    {
      throw new NotImplementedException();
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
      return new PersonUpdateRequest() { PersonID = PersonID, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!, true), CountryID = CountryID, Address = Address, ReceiveNewsLetters = ReceiveNewsLetters };
    }
  }

  public static class PersonExtensions
  {
    public static PersonResponse ToPersonResponse(this Person person)
    {
      return new PersonResponse() { PersonID = person.PersonID, PersonName = person.PersonName, Email = person.Email, DateOfBirth = person.DateOfBirth, Gender = person.Gender, CountryID = person.CountryID, Address = person.Address, ReceiveNewsLetters = person.ReceiveNewsLetters, Age = (person.DateOfBirth != null) ? Math.Floor((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null, Country = person.Country?.CountryName };
    }
  }
}

