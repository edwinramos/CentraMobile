using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srTable")]
    public class DeTable
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string KeyFixed { get; set; }

        public string KeyVariable { get; set; }

        public string Value { get; set; }
    }
}
