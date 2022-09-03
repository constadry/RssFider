using System.Net;
using System.Xml;
using RssFider.Models;

namespace RssFider.Services;

public class HabrBot
{
    public static List<Article> GetArticles()
    {
        var articles = new List<Article>();
        var doc = GetXml();

        var nodes = doc.DocumentElement?.SelectNodes("/channel/item");
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

    private static XmlDocument GetXml()
    {
        var doc = new XmlDocument();
        try
        {
            var url = "https://habr.com/ru/rss/interesting/";
            var proxyUrl = "http://xxx.xxx.x.x:8080/";

            var wp = new WebProxy(proxyUrl)
            {
                Credentials = CredentialCache.DefaultCredentials
            };
            var wc = new WebClient();
            wc.Proxy = wp;

            var ms = new MemoryStream(wc.DownloadData(url));
            var rdr = new XmlTextReader(ms);

            doc.Load(rdr);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return doc;
    } 
}