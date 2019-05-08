using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srPriceList")]
    public class DePriceList
    {
        [PrimaryKey]
        public string PriceListCode { get; set; }

        public string PriceListDescription { get; set; }
    }
}
