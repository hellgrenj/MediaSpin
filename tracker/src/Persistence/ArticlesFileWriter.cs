using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Serilog;
using tracker.Domain.Ports.Outbound;
using tracker.Domain.Models;

namespace tracker.Persistence
{
    public class ArticlesFileWriter : IArticlesFileWriter
    {
        public void WriteToJSON(List<Article> articles)
        {
            Log.Information($"writing to json file all {articles.Count()} articles");
            string json = JsonConvert.SerializeObject(articles.ToArray(), Formatting.Indented);
            System.IO.File.WriteAllText(@"/files/articles.json", json, Encoding.UTF8);
            Log.Debug($"Lenght of json {json.Length}");
        }
    }
}