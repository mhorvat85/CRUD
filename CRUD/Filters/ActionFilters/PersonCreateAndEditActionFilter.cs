using CRUD.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD.Filters.ActionFilters
{
  public class PersonCreateAndEditActionFilter : IAsyncActionFilter
  {
    private readonly ICountriesService _countriesService;

    public PersonCreateAndEditActionFilter(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      if (context.Controller is PersonsController personsController)
      {
        if (!personsController.ModelState.IsValid)
        {
          List<CountryResponse> countries = await _countriesService.GetAllCountries();
          personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

          personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

          var personRequest = context.ActionArguments["personRequest"];

          context.Result = personsController.View(personRequest);
        }
        else
        {
          await next();

          if (context.HttpContext.Request.Method == "GET")
          {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() }).OrderBy(temp => temp.Text);
          }
        }
      }
      else
      {
        await next();
      }
    }
  }
}
