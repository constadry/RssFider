using System.Net;
using System.Xml;
using RssFider.Models;

namespace RssFider.Services;

public class HabrBot : IHabrBot
{
    private readonly IConfiguration _configuration;

    public HabrBot(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Article> GetArticles()
    {
        var articles = new List<Article>();
        var doc = GetXml();

        var nodes = doc.DocumentElement?.SelectNodes("/rss/channel/item");
        foreach (XmlNode node in nodes)
        {
            var title = node?.SelectSingleNode("title")?.InnerText ?? string.Empty;
            var url = node?.SelectSingleNode("guid")?.InnerText ?? string.Empty;
            var description = node?.SelectSingleNode("description")?.InnerText ?? string.Empty;
            var releaseDate = node?.SelectSingleNode("pubDate")?.InnerText ?? string.Empty;
            
            articles.Add(
                    new Article
                    {
                        Title = title,
                        Url = url,
                        Description = description,
                        ReleaseDate = releaseDate,
                    }
            );
        }

        return articles;
    }

    private XmlDocument GetXml()
    {
        var httpClient = new HttpClient();
        var host = _configuration["proxy:host"] ?? string.Empty;
        var port = _configuration["proxy:port"] ?? string.Empty;
        var userName = _configuration["proxy:userName"] ?? string.Empty;
        var password = _configuration["proxy:password"] ?? string.Empty;
        
        // First create a proxy object
        var proxy = new WebProxy
        {
            Address = new Uri($"http://{host}:{port}"),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,

            // *** These creds are given to the proxy server, not the web server ***
            Credentials = new NetworkCredential(
                userName: userName,
                password: password)
        };

        // Now create a client handler which uses that proxy
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
        };

        if (_configuration["useProxy"] == "true")
        {
            httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }

        var doc = new XmlDocument();
        try
        {
            var url = _configuration["feed"];
            var result = httpClient.GetAsync(url).Result;
            var xml = result.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(xml))
            {
                Console.WriteLine("something wrong");
                return new XmlDocument();
            }

            doc.LoadXml(xml);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return doc;
    } 
}