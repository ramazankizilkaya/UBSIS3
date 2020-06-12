using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UBSIS3.Web.Data.Dtos
{
    public class VacancyDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string WhatWeExpectFromYou { get; set; }
    }
}
