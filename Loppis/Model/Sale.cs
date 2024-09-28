using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace loppis.Model;

public sealed class Sale
{
    public string Cashier { get; set; }
    public DateTime Timestamp { get; set; }
    public string TimestampString
    {
        get
        {
            return Timestamp.ToShortTimeString();
        }
    }
    public List<SaleEntry> Entries { get; private set; } = [];
    public int SumTotal => Entries.Sum((x) => x.Price.GetValueOrDefault());

    public SaleEntry this[int i]
    {
        get { return Entries[i]; }
        set { Entries[i] = value; }
    }
    public Sale()
    {
    }

    public Sale(ObservableCollection<SaleEntry> inputList, string cashier)
    {
        Cashier = cashier;
        Timestamp = DateTime.Now;
        foreach (var entry in inputList)
        {
            Entries.Add(entry);
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is not Sale other)
        {
            return false;
        }
        return Cashier == other.Cashier && Timestamp == other.Timestamp && Entries.SequenceEqual(other.Entries);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Cashier, Timestamp);
    }
}
