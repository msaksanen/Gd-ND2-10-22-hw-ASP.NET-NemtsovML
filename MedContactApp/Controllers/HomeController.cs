using MedContactApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MedContactApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CustomException(string? exceptionStack, string? exceptionMessage)
        {
            CustomExceptionModel model = new();
            if (!string.IsNullOrEmpty(exceptionStack))
                model.ExceptionStack = exceptionStack;
            if (!string.IsNullOrEmpty(exceptionMessage))
                model.ExceptionMessage = exceptionMessage;
            return View(model);
        }

        [HttpGet]
        public IActionResult CustomStatus(int? code, string? name, string? text)
        {
            CustomStatusModel model = new();
            if (code != null)
            {
                model.Code = code;
                if (code == 404)
                    model.Status = String.Join(" ", "Status", code, "(Not Found)");
                if (code == 400)
                    model.Status = String.Join(" ", "Status", code, "(Bad Request)");
            }

            if (!string.IsNullOrEmpty(name))
                model.Name = name;
            if (!string.IsNullOrEmpty(text))
                model.Text = text;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}