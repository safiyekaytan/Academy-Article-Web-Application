using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using yazlab2_1.Models;



namespace yazlab2_1.Data
{
    public class ElasticsearchService
    {
        private readonly ElasticClient _client;

        public ElasticsearchService(Uri elasticsearchUri)
        {
            var settings = new ConnectionSettings(elasticsearchUri);
            _client = new ElasticClient(settings);
        }

        public async Task<List<Article>> GetDataAsync()
        {
            var searchResponse = await _client.SearchAsync<Article>(s => s
                .Index("ArticleCollection4") // Elasticsearch'te kullanılan index'in adı
                .Query(q => q.MatchAll()) // Tüm belgeleri almak için sorgu
                .Size(1000)); // İsteğe bağlı olarak alınacak belge sayısı

            if (!searchResponse.IsValid)
            {
                // Hata işleme
                throw new Exception($"Elasticsearch sorgusu başarısız: {searchResponse.DebugInformation}");
            }

            return searchResponse.Documents.ToList();
        }
    }
}
