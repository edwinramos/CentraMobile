using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srItem")]
    public class DeItem
    {
        [PrimaryKey]
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Barcode { get; set; }

        public double TaxValue { get; set; }

        public string DepartmentCode { get; set; }
    }
}
