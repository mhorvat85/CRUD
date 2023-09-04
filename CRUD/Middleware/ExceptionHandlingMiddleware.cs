namespace CRUD.Middleware
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
      try
      {
        await _next(httpContext);
      }
      catch 
      {
        //httpContext.Response.StatusCode = 500;
        //await httpContext.Response.WriteAsync("Error occurred");
        throw;
      }
    }
  }

  public static class ExceptionHandlingMiddlewareExtensions
  {
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
  }
}
