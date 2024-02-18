using System.Xml.Serialization;

namespace loppis.Model;

public class Seller
{
    [XmlIgnore]
    public string Id { get; set; }
    public string Name { get; set; }
    public int? DefaultPrice { get; set; }
}
