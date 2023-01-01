

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBExample.Entities
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public bool Status { get; set; }

        protected EntityBase()
        {
            Status = true;
        }
    }
}
