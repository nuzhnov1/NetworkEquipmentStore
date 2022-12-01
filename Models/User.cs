using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkEquipmentStore.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public PermissionsLevel Level { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
    }
}
