using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Data.Dtos
{
    public class CareerDto
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Please enter max 50 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address."), Required(ErrorMessage = "Please enter your email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select position you want to apply.")]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Web Site")]
        public string WebSite { get; set; }

        [MaxLength(500, ErrorMessage = "Please enter max 500 characters.")]
        [Display(Name = "Motivation Letter")]
        public string MotivationLetter { get; set; }

        [Range(1000, 20000, ErrorMessage = "Please enter a value between 1.000 and 20.000")]
        [Display(Name = "Salary")]
        public int? Salary { get; set; }

        [Display(Name="Salary Currency")]
        public string SalaryCurrency { get; set; }

        public string ResumeFileName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please attach your resume.")]
        [Display(Name= "UPLOAD YOUR RESUME")]
        public IFormFile Resume { get; set; }
    }
}
