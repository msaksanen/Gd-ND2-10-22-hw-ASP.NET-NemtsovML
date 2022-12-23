using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MedContactApp.Filters
{
    public class StatusCodeResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // retrieve a typed controller, so we can reuse its data
            if (context.Controller is Controller controller)
            {
                // intercept the NotFoundObjectResult
                if (context.Result is NotFoundObjectResult notFoundResult)
                {
                    context.Result = new RedirectToActionResult("CustomStatus", "Home",
                                    new { code = notFoundResult.StatusCode, name = context.ActionDescriptor.DisplayName, 
                                          text = notFoundResult.Value });  

                }

                if (context.Result is NotFoundResult notFound)
                {
                    context.Result = new RedirectToActionResult("CustomStatus", "Home",
                                    new
                                    {
                                        code = notFound.StatusCode,
                                        name = context.ActionDescriptor.DisplayName
                                    });

                }

                if (context.Result is BadRequestObjectResult badRequestResult)
                {
                    context.Result = new RedirectToActionResult("CustomStatus", "Home",
                                    new
                                    {
                                        code = badRequestResult.StatusCode,
                                        name = context.ActionDescriptor.DisplayName,
                                        text = badRequestResult.Value
                                    });

                }

            }

            await next();
        }
    }
}
