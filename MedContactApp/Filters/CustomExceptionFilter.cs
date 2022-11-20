using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MedContactApp.Filters
{
    public class CustomExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            string? exceptionStack = context.Exception.StackTrace;
            string exceptionMessage = context.Exception.Message;
            context.Result = new RedirectToActionResult("CustomException", "Home", 
                                  new {exceptionStack = exceptionStack, exceptionMessage=exceptionMessage });  
            context.ExceptionHandled = true;
                
       }
    }
}
