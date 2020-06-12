using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Data.Context;
using UBSIS3.Web.Data.Interfaces;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Repositories
{
    public class CareerRepository : Repository<Career>, ICareerRepository
    {
        public CareerRepository(ApplicationContext context) : base(context) { }
    }
}
