using CRUD.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD.Filters.ActionFilters
{
  public class PersonsListActionFilter : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      if (context.ActionArguments.ContainsKey("searchBy"))
      {
        string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

        if (!string.IsNullOrEmpty(searchBy))
        {
          List<string> searchByOptions = new()
          {
            nameof(PersonResponse.PersonName),
            nameof(PersonResponse.Email),
            nameof(PersonResponse.DateOfBirth),
            nameof(PersonResponse.Gender),
            nameof(PersonResponse.CountryID),
            nameof(PersonResponse.Address)
          };

          if (searchByOptions.Any(temp => temp == searchBy) == false)
          {
            context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
          }
        }
      }

      if (context.ActionArguments.ContainsKey("sortBy"))
      {
        string? sortBy = Convert.ToString(context.ActionArguments["sortBy"]);

        if (!string.IsNullOrEmpty(sortBy))
        {
          List<string> sortByOptions = new()
          {
            nameof(PersonResponse.PersonName),
            nameof(PersonResponse.Email),
            nameof(PersonResponse.DateOfBirth),
            nameof(PersonResponse.Age),
            nameof(PersonResponse.Gender),
            nameof(PersonResponse.CountryID),
            nameof(PersonResponse.Address),
            nameof(PersonResponse.ReceiveNewsLetters),
          };

          if (sortByOptions.Any(temp => temp == sortBy) == false)
          {
            context.ActionArguments["sortBy"] = nameof(PersonResponse.PersonName);
          }
        }
      }

      if (context.ActionArguments.ContainsKey("sortOrder"))
      {
        string? sortOrder = Convert.ToString(context.ActionArguments["sortOrder"]);

        if (!string.IsNullOrEmpty(sortOrder))
        {
          List<string> sortOrdersOptions = new()
          {
            SortOrderOptions.ASC.ToString(),
            SortOrderOptions.DESC.ToString(),
          };

          if (sortOrdersOptions.Any(temp => temp == sortOrder) == false)
          {
            context.ActionArguments["sortOrder"] = SortOrderOptions.ASC;
          }
        }
      }

      context.HttpContext.Items["arguments"] = context.ActionArguments;

      await next();

      PersonsController personsController = (PersonsController)context.Controller;

      personsController.ViewBag.SearchFields = new Dictionary<string, string>()
      {
        { nameof(PersonResponse.PersonName), "Person Name" },
        { nameof(PersonResponse.Email), "Email" },
        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        { nameof(PersonResponse.Gender), "Gender" },
        { nameof(PersonResponse.CountryID), "Country" },
        { nameof(PersonResponse.Address), "Address" },
      };

      IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

      if (parameters != null)
      {
        if (parameters.ContainsKey("searchBy"))
        {
          personsController.ViewBag.CurrentSearchBy = parameters["searchBy"];
        }

        if (parameters.ContainsKey("searchString"))
        {
          personsController.ViewBag.CurrentSearchString = parameters["searchString"];
        }

        if (parameters.ContainsKey("sortBy"))
        {
          personsController.ViewBag.CurrentSortBy = parameters["sortBy"];
        }

        if (parameters.ContainsKey("sortOrder"))
        {
          personsController.ViewBag.CurrentSortOrder = Convert.ToString(parameters["sortOrder"]);
        }
      }
    }
  }
}
