using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Models
{
    public class Career:BaseModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 chacracters.")]
        [MaxLength(50, ErrorMessage = "Please enter max 50 chacracters.")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address."), Required(ErrorMessage = "Please enter your e-mail address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select position you want to apply.")]
        public string Position { get; set; }

        [DataType(DataType.Url)]
        public string WebSite { get; set; }

        [MaxLength(500, ErrorMessage = "Please enter max 500 chacracters.")]
        public string MotivationLetter { get; set; }

        [Range(1000, 20000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Salary { get; set; }

        public string SalaryCurrency { get; set; }

        public string ResumeFileName { get; set; }

    }
}
