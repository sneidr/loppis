﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace loppis.Model
{
    public class Sale
    {
        List<SaleEntry> entries = new();
        public string Cashier { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimestampString
        {
            get
            {
                return Timestamp.ToShortTimeString();
            }
        }
        public List<SaleEntry> Entries { get => entries; set => entries = value; }
        public int SumTotal => Entries.Sum((x) => x.Price.GetValueOrDefault());

        public SaleEntry this[int i]
        {
            get { return entries[i]; }
            set { entries[i] = value; }
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
    }
}
