using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Prism.Mvvm;
using System;
using System.Xml.Serialization;

namespace DataAccess.Model;

public class SaleEntry : BindableBase, IEquatable<SaleEntry>
{
    [XmlIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public const int RoundUpId = 999;
    public static int? CardId { get; set; }
    public static int? BagId { get; set; }

    public SaleEntry() : this(null, null)
    {
    }

    public SaleEntry(int? id, int? price)
    {
        m_sellerId = id;
        m_price = price;
    }

    public void Clear()
    {
        SellerId = null;
        Price = null;
    }

    private int? m_sellerId;
    private int? m_price;

    public int? SellerId { get => m_sellerId; set { SetProperty(ref m_sellerId, value); } }
    public int? Price { get => m_price; set { SetProperty(ref m_price, value); } }

    [XmlIgnoreAttribute]
    public string SellerIdListText
    {
        get
        {
            if (m_sellerId == CardId)
            {
                return "Vykort      ";
            }
            else if (m_sellerId == BagId)
            {
                return "Kasse       ";
            }
            else if (m_sellerId == RoundUpId)
            {
                return "Avrundning  ";
            }
            else
            {
                return $"Säljare: {m_sellerId,3}";
            }
        }
    }

    [XmlIgnoreAttribute]
    public string PriceListText
    {
        get
        {
            string text = string.Format("{0,3}", m_price);
            return text;
        }
    }

    public bool Equals(SaleEntry other)
    {
        if (other == null)
        { return false; }

        if (m_sellerId == other.SellerId && m_price == other.Price)
        { return true; }
        else
        {
            return false;
        }

    }
}
