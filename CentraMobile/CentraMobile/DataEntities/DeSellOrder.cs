using System.ComponentModel;
using System.Collections.Generic;
using SQLite;
using System;
using System.Runtime.CompilerServices;

namespace CentraMobile.DataEntities
{
    [Table("srSellOrder")]
    public class DeSellOrder
    {
        [PrimaryKey]
        public int SellOrderId { get; set; }

        public string PriceListCode { get; set; }

        public DateTime DocDateTime { get; set; }

        public string ClientCode { get; set; }

        public string ClientDescription { get; set; }

        public string ExternalReference { get; set; }

        public double VatSum { get; set; }

        public double TotalDiscount { get; set; }

        public double DocTotal { get; set; }

        public bool IsClosed { get; set; }

        public string Comments { get; set; }

        public string PaymentTypeCode { get; set; }

        public string StoreCode { get; set; }

        public string WarehouseCode { get; set; }

        public double DocNetTotal { get; set; }

        public DateTime ClosedDateTime { get; set; }

        public string StorePosCode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
