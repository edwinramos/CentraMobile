using System.ComponentModel;
using System.Collections.Generic;
using SQLite;
using System;

namespace CentraMobile.DataEntities
{
    [Table("srSellOrderDetail")]
    public class DeSellOrderDetail
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int SellOrderId { get; set; }

        public int LineNumber { get; set; }
        
        public bool IsClosed { get; set; }
        
        public string ItemCode { get; set; }
        
        public string ItemDescription { get; set; }

        public string Barcode { get; set; }

        public string ExternalCode { get; set; }
        
        public double PriceBefDiscounts { get; set; }

        public double DiscountValue { get; set; }
        
        public double Quantity { get; set; }

        public double Price { get; set; }
        
        public string WarehouseCode { get; set; }

        
        public string VatCode { get; set; }

        
        public double VatPercent { get; set; }

        
        public double VatValue { get; set; }

        
        public double PriceAftVat { get; set; }

        
        public double TotalRowValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
