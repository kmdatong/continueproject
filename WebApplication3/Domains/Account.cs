using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Domains
{
    [Table("dtuser")]
    public class Account
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public string Password { get; set; }

        public string LoginName { get; set; }
    }
}
