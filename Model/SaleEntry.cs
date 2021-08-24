using Prism.Mvvm;
using System;

namespace loppis.Model
{
    public class SaleEntry : BindableBase, IEquatable<SaleEntry>
    {

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

        public int? SellerId
        {
            get => m_sellerId;

            set
            {
                SetProperty(ref m_sellerId, value);
            }
        }

        public int? Price
        {
            get => m_price;

            set
            {
                SetProperty(ref m_price, value);
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
