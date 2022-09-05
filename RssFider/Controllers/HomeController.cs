using Microsoft.AspNetCore.Mvc;
using RssFider.Services;

namespace RssFider.Controllers;

public class HomeController : Controller
{
    private readonly IHabrBot _habrBot;
    private readonly IConfiguration _configuration;

    public HomeController(IHabrBot habrBot, IConfiguration configuration)
    {
        _habrBot = habrBot;
        _configuration = configuration;
    }
    
    // GET
    public IActionResult Index()
    {
        ViewData["FeedUpdateTimeout"] = _configuration["updateTimeout"] ?? "500";
        ViewData["AddMarkupDescription"] = _configuration["addMarkupDescription"] ?? "false";
        var articles = _habrBot.GetArticles();
        return View(articles);
    }
}