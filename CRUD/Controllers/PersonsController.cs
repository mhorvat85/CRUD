using CRUD.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD.Controllers
{
  [Route("[controller]")]
  public class PersonsController : Controller
  {
    private readonly IPersonsService _personsService;

    public PersonsController(IPersonsService personsService)
    {
      _personsService = personsService;
    }

    [Route("/")]
    [Route("[action]")]
    [TypeFilter(typeof(PersonsListActionFilter))]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC )
    {
      List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

      List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);

      return View(sortedPersons);
    }

    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
    public IActionResult Create()
    {
      return View();
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
    public async Task<IActionResult> Create(PersonAddRequest personRequest)
    {
      await _personsService.AddPerson(personRequest);

      return RedirectToAction("Index", "Persons");
    }

    [Route("[action]/{personID}")]
    [HttpGet]
    [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
    public async Task<IActionResult> Edit(Guid personID)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

      return View(personUpdateRequest);
    }

    [Route("[action]/{personID}")]
    [HttpPost]
    [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
    public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personRequest.PersonID);

      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }
      
      await _personsService.UpdatePerson(personRequest);

      return RedirectToAction("Index");
    }

    [Route("[action]/{personID}")]
    [HttpGet]
    public async Task<IActionResult> Delete(Guid? personID)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      return View(personResponse);
    }

    [Route("[action]/{personID}")]
    [HttpPost]
    public async Task<IActionResult> Delete(Guid personID)
    {
      PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

      if (personResponse == null)
      {
        return RedirectToAction("Index");
      }

      await _personsService.DeletePerson(personResponse.PersonID);

      return RedirectToAction("Index");
    }
  }
}
