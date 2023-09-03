using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Repositories
{
  public class PersonsRepository : IPersonsRepository
  {
    private readonly ApplicationDbContext _db;

    public PersonsRepository(ApplicationDbContext db)
    {
      _db = db;
    }

    public async Task<Person> AddPerson(Person person)
    {
      _db.Persons.Add(person);
      await _db.SaveChangesAsync();

      return person;
    }

    public async Task<List<Person>> GetAllPersons()
    {
      return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonID(Guid? personID)
    {
      return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
      return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
    }

    public async Task<Person> UpdatePerson(Person? person)
    {
      Person? matchingPerson = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == person!.PersonID);

      if (matchingPerson == null) return person!;

      foreach (PropertyInfo prop in matchingPerson.GetType().GetProperties())
      {
        if (prop.Name != nameof(Person.PersonID) && prop.Name != nameof(Person.Country))
        {
          string propertyName = prop.Name;
          var otherPropValue = person!.GetType().GetProperty(propertyName)?.GetValue(person);

          if (propertyName == nameof(Person.Gender)) otherPropValue = otherPropValue?.ToString();

          matchingPerson.GetType().GetProperty(propertyName)?.SetValue(matchingPerson, otherPropValue);
        }
      }
      await _db.SaveChangesAsync();

      return matchingPerson;
    }

    public async Task<bool> DeletePerson(Guid? personID)
    {
      _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personID));
      int rowsDeleted = await _db.SaveChangesAsync();

      return rowsDeleted > 0;
    }
  }
}
