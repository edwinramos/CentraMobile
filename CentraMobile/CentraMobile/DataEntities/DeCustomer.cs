using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srCustomer")]
    public class DeCustomer
    {
        [PrimaryKey]
        public string BusinessPartnerCode { get; set; }

        public string BusinessPartnerDescription { get; set; }

        public string RNC { get; set; }

        public string PriceListCode { get; set; }
    }
}
