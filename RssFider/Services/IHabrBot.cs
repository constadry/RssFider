using RssFider.Models;

namespace RssFider.Services;

public interface IHabrBot
{
    List<Article> GetArticles();
}