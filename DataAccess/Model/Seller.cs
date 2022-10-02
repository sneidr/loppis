using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Xml.Serialization;

namespace DataAccess.Model;

public class Seller
{
    [XmlIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Name { get; set; }
    public int? DefaultPrice { get; set; }
}
