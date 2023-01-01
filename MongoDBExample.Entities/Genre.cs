using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBExample.Entities
{
    public class Genre : EntityBase
    {
        public string Name { get; set; } = null!;
    }
}
