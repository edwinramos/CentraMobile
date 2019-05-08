using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srPrice")]
    public class DePrice
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string PriceListCode { get; set; }

        public string ItemCode { get; set; }

        public double SellPrice { get; set; }
    }
}
