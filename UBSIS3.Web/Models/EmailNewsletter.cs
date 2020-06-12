using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Models
{
    public class EmailNewsletter:BaseModel
    {
        [EmailAddress(ErrorMessage = "Please enter a valid email address."), Required(ErrorMessage = "Please enter your email address."),]
        public string EmailAddress { get; set; }

        public string UserIp { get; set; }
    }
}
