using MongoDB.Bson;
using MongoDB.Driver;
using yazlab2_1.Models;

namespace yazlab2_1.Data
{
    public class MongoDBD
    {
        private IMongoCollection<Article> _collection;
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "Article";
        string collectionName = "ArticleCollection4";
        public MongoDBD()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<Article>(collectionName);
        }

        public async Task<List<Article>> GetDataAsync()
        {
            var filter = Builders<Article>.Filter.Empty;
            var documents = await _collection.Find(filter).ToListAsync();
            return documents;
        }

        public async Task InsertDataAsync(Article document)
        {
            await _collection.InsertOneAsync(document);
        }
    }
}
