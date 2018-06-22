using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class AccountModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("年龄")]
        public int Age { get; set; }

        [Required]
        [DisplayName("姓名")]
        public string Name { get; set; }

        [Required]
        [DisplayName("备注")]
        public string Remark { get; set; }

        [Required]
        [DisplayName("密码")]
        public string Password { get; set; }

        [Required]
        [DisplayName("登录名")]
        public string LoginName { get; set; }
    }
}
