using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Models
{
    public class ErrorLog:BaseModel
    {
        public string ErrorDetail { get; set; }
        public string ErrorLocation { get; set; }
        public int StatusCode { get; set; }
    }
}
