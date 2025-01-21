using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Todoo.App.Models;
using Todoo.App.Services;

namespace Todoo.App.Controllers;

public class HomeController : Controller
{
    private readonly ITodoApiService _service;

    public HomeController(ITodoApiService service)
    {
        _service = service;
    }
    
    public async Task<IActionResult> Index()
    {
        var todoItems = await _service.GetTodoItemsAsync();
        return View(todoItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewItem(CreateTodoItem model)
    {
        await _service.CreateTodoItemAsync(model);
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Logout()
    {
        return SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}