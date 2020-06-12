using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Data.Dtos;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Mapper
{
    public class UbsisMapper:Profile
    {
        public UbsisMapper()
        {
            CreateMap<ContactUs, ContactUsDto>().ReverseMap();
            CreateMap<Career, CareerDto>().ReverseMap();
        }
    }
}
