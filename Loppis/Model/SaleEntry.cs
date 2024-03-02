using Prism.Mvvm;
using System;
using System.Xml.Serialization;

namespace loppis.Model;

public sealed class SaleEntry(int? id, int? price) : BindableBase, IEquatable<SaleEntry>
{
    [XmlIgnore]
    public string Id { get; set; }

    public const int RoundUpId = 999;
    public static int? CardId { get; set; }
    public static int? BagId { get; set; }

    public SaleEntry() : this(null, null)
    {
    }

    public void Clear()
    {
        SellerId = null;
        Price = null;
    }

    public int? SellerId { get => id; set { SetProperty(ref id, value); } }
    public int? Price { get => price; set { SetProperty(ref price, value); } }

    [XmlIgnoreAttribute]
    public string SellerIdListText
    {
        get
        {
            if (id == CardId)
            {
                return "Vykort      ";
            }
            else if (id == BagId)
            {
                return "Kasse       ";
            }
            else if (id == RoundUpId)
            {
                return "Avrundning  ";
            }
            else
            {
                return $"Säljare: {id,3}";
            }
        }
    }

    [XmlIgnoreAttribute]
    public string PriceListText
    {
        get
        {
            string text = string.Format("{0,3}", price);
            return text;
        }
    }

    public bool Equals(SaleEntry other)
    {
        if (other == null)
        { return false; }

        if (id == other.SellerId && price == other.Price)
        { return true; }
        else
        {
            return false;
        }

    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SaleEntry);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 27;
            hash = (13 * hash) + SellerId.GetHashCode();
            hash = (13 * hash + Price.GetHashCode());
            return hash;
        }
    }
}
