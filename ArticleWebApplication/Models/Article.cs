using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

namespace yazlab2_1.Models
{
    public class Article
    {
        public ObjectId Id { get; set; } // _id alanı
        public String publishName { get; set; }
        public String authorsName { get; set; }
        public String publishType { get; set; }
        public DateTime publishDate { get; set; }
        public String publishArea { get; set; }
        public String keywordsSearched { get; set; }
        public String keywordsArticle { get; set; }
        public String summary { get; set; }
        public List<string> references { get; set; }
        public String numberOfCitations { get; set; }
        public String doiNumber { get; set; }
        public String urlAddress { get; set; }
        public String downloadLink { get; set; }




    }
}
