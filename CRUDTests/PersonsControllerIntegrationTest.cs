using FluentAssertions;
using HtmlAgilityPack;

namespace CRUDTests
{
  public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
  {
    private readonly HttpClient _httpClient;

    public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
      _httpClient = factory.CreateClient();
    }

    #region Index
    [Fact]
    public async Task Index_ToReturnView()
    {
      HttpResponseMessage response = await _httpClient.GetAsync("/Persons/Index");

      response.Should().BeSuccessful();

      string responseBody = await response.Content.ReadAsStringAsync();
      HtmlDocument html = new();
      html.LoadHtml(responseBody);
      string documentTitle = html.DocumentNode.SelectSingleNode("//title").InnerHtml;

      documentTitle.Should().Be("Persons");
    }
    #endregion
  }
}
