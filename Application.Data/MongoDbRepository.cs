using MongoDB.Driver;
using System.Threading.Tasks;

namespace Application.Data
{
    public class MongoDbRepository<TDocument> : IRepository<TDocument>
        where TDocument : class
    {
        public MongoDbRepository(string collectionName, IMongoDatabase database)
        {
            Collection = database.GetCollection<TDocument>(collectionName);
        }

        protected IMongoCollection<TDocument> Collection { get; }

        public void Add(TDocument document)
        {
            Collection.InsertOne(document);
        }
    }
}
