using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace loppis.Model
{
    public class Sale<T>
    {
        List<T> entries = new();
        public string Cashier { get; set; }
        public DateTime Timestamp { get; set; }
        public List<T> Entries { get => entries; set => entries = value; }

        public T this[int i]
        {
            get { return entries[i]; }
            set { entries[i] = value; }
        }
        public Sale()
        {
        }

        public Sale(ObservableCollection<T> inputList, string cashier)
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
