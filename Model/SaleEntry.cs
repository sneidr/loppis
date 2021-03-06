using Prism.Mvvm;
using System;
using System.Xml.Serialization;

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

        public int? SellerId { get => m_sellerId; set { SetProperty(ref m_sellerId, value); } }
        public int? Price { get => m_price; set { SetProperty(ref m_price, value); } }

        [XmlIgnoreAttribute]
        public string SellerIdListText
        {
            get
            {
                if (m_sellerId == loppis.ViewModels.SalesViewModel.CardId)
                {
                    return "Vykort      ";
                }
                else if (m_sellerId == loppis.ViewModels.SalesViewModel.BagId)
                {
                    return "Kasse       ";
                }
                else if (m_sellerId == loppis.ViewModels.SalesViewModel.RoundUpId)
                {
                    return "Avrundning  ";
                }
                else
                {
                    return $"Säljare: {m_sellerId.ToString(),3}";
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
}
