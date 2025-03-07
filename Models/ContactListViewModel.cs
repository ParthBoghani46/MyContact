using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyContact.Models
{
    public class ContactListViewModel
    {
        public t_Contact contact { get; set; }
        public string c_cityname { get; set; }
        public string c_statename { get; set; }
        public string c_StatusName { get; set; }

    }
}