using AutoFixture;
using CRUD.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
  public class PersonsControllerTest
  {
    private readonly PersonsController _personsController;
    private readonly IPersonsService _personsService;
    private readonly Mock<IPersonsService> _personsServiceMock;
    private readonly IFixture _fixture;

    public PersonsControllerTest()
    {
      _fixture = new Fixture();
      _personsServiceMock = new Mock<IPersonsService>();
      _personsService = _personsServiceMock.Object;
      _personsController = new PersonsController(_personsService);
    }

    #region Index
    [Fact]
    public async Task Index_ToReturnIndexViewWithPersonsList()
    {
      //Arrange
      List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

      _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons_response_list);
      _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).Returns(persons_response_list);

      //Act
      IActionResult result = await _personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

      //Assert
      ViewResult view_result = Assert.IsType<ViewResult>(result);

      view_result.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
      view_result.ViewData.Model.Should().Be(persons_response_list);
    }
    #endregion

    #region Create
    [Fact]
    public void Create_ToReturnCreateView_Get()
    {
      //Act
      IActionResult result = _personsController.Create();

      //Assert
      Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_ToRedirectToAction_Post()
    {
      //Arrange
      PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
      PersonResponse person_response = person_add_request.ToPerson().ToPersonResponse();

      _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Create(person_add_request);

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }
    #endregion

    #region Edit
    [Fact]
    public async Task Edit_IfNullPerson_ToRedirectToAction_Get()
    {
      //Arrange
      PersonResponse? person_response = null;

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Edit(_fixture.Create<Guid>());

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Edit_ToReturnEditViewWithPerson_Get()
    {
      //Arrange
      PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
      PersonResponse person_response = person_add_request.ToPerson().ToPersonResponse();
      PersonUpdateRequest person_update_request = person_response.ToPersonUpdateRequest();

      List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Edit(_fixture.Create<Guid>());

      //Assert
      ViewResult view_result = Assert.IsType<ViewResult>(result);

      view_result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
      view_result.ViewData.Model.Should().BeEquivalentTo(person_update_request);
    }

    [Fact]
    public async Task Edit_IfNullPerson_ToRedirectToAction_Post()
    {
      //Arrange
      PersonUpdateRequest person_update_request = _fixture.Create<PersonUpdateRequest>();
      PersonResponse? person_response = null;

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Edit(person_update_request);

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Edit_ToRedirectToAction_Post()
    {
      //Arrange
      PersonUpdateRequest person_update_request = _fixture.Create<PersonUpdateRequest>();
      PersonResponse person_response = person_update_request.ToPerson().ToPersonResponse();

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);
      _personsServiceMock.Setup(temp => temp.UpdatePerson(It.IsAny<PersonUpdateRequest>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Edit(person_update_request);

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_IfNullPerson_ToRedirectToAction_Get()
    {
      //Arrange
      PersonResponse? person_response = null;

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Delete(_fixture.Create<Guid?>());

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Delete_ToReturnDeleteViewWithPerson_Get()
    {
      //Arrange
      PersonResponse person_response = _fixture.Create<PersonResponse>();

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Delete(_fixture.Create<Guid?>());

      //Assert
      ViewResult view_result = Assert.IsType<ViewResult>(result);

      view_result.ViewData.Model.Should().BeAssignableTo<PersonResponse>();
      view_result.ViewData.Model.Should().BeEquivalentTo(person_response);
    }

    [Fact]
    public async Task Delete_IfNullPerson_ToRedirectToAction_Post()
    {
      //Arrange
      PersonResponse? person_response = null;

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Delete(_fixture.Create<Guid>());

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Delete_ToRedirectToAction_Post()
    {
      //Arrange
      PersonResponse person_response = _fixture.Create<PersonResponse>();

      _personsServiceMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_response);

      //Act
      IActionResult result = await _personsController.Delete(_fixture.Create<Guid>());

      //Assert
      RedirectToActionResult redirect_action_result = Assert.IsType<RedirectToActionResult>(result);
      redirect_action_result.ActionName.Should().Be("Index");
    }
    #endregion
  }
}
