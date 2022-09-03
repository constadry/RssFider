using Microsoft.AspNetCore.Mvc;
using RssFider.Services;

namespace RssFider.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        var articles = HabrBot.GetArticles();
        return View(articles);
    }
}