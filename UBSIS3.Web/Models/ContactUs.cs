using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Models
{
    public class ContactUs:BaseModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must have min length of 2 and max length of 200")]
        public string Name { get; set; }

        [Display(Name= "Company Name")]
        [StringLength(200, ErrorMessage = "Please enter max 200 characters")]
        public string CompanyName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address."), Required(ErrorMessage = "Please enter your e-mail address.")]
        [StringLength(200, ErrorMessage = "Please enter max 200 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter subject.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Subject must have min length of 2 and max length of 200")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(4000, MinimumLength = 20, ErrorMessage = "Message must have min length of 20 and max length of 4000")]
        public string Message { get; set; }
    }
}
