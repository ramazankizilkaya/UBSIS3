using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Data.Dtos
{
    public class ContactUsDto
    {
        [Required(ErrorMessage = "Please enter your name")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters")]
        [MaxLength(200, ErrorMessage = "Please enter max 200 characters")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Company Name")]
        [MaxLength(200, ErrorMessage = "Please enter max 200 characters")]
        public string CompanyName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address"), Required(ErrorMessage = "Please enter your e-mail address")]
        [MaxLength(200, ErrorMessage = "Please enter max 200 characters")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter subject")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters")]
        [MaxLength(200, ErrorMessage = "Please enter max 200 characters")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message")]
        [MinLength(20, ErrorMessage = "Please enter at least 20 characters")]
        [MaxLength(4000, ErrorMessage = "Please enter max 4000 characters")]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Please enter security code")]
        public int SecurityCode { get; set; }
    }
}
