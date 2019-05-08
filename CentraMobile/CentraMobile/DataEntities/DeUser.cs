using System.ComponentModel;
using System.Collections.Generic;
using SQLite;

namespace CentraMobile.DataEntities
{
    [Table("srUsers")]
    public class DeUser
    {
        [PrimaryKey]
        public string UserCode { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        HOMBRE = 0,
        MUJER = 1
    }
}
