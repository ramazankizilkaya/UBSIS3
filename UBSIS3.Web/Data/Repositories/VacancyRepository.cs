using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UBSIS3.Web.Data.Context;
using UBSIS3.Web.Data.Interfaces;
using UBSIS3.Web.Models;

namespace UBSIS3.Web.Data.Repositories
{
    public class VacancyRepository : Repository<Vacancy>, IVacancyRepository
    {
        public VacancyRepository(ApplicationContext context) : base(context) { }
    }
}
