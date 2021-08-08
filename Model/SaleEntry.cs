using System;
using System.ComponentModel;

namespace loppis.Model
{
    public class SaleEntry : INotifyPropertyChanged, IEquatable<SaleEntry>
    {

        public SaleEntry() : this(0, 0)
        {
        }

        public SaleEntry(int id, int price)
        {
            m_sellerId = id;
            m_price = price;
        }

        public void Clear()
        {
            m_sellerId = 0;
            m_price = 0;
        }

        private int m_sellerId;
        private int m_price;

        public int SellerId
        {
            get => m_sellerId;

            set
            {
                if (m_sellerId != value)
                {
                    m_sellerId = value;
                    RaisePropertyChanged("SellerId");
                }
            }
        }

        public int Price
        {
            get => m_price;

            set
            {
                if (m_price != value)
                {
                    m_price = value;
                    RaisePropertyChanged("Price");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
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
}
