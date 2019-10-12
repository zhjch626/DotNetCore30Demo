using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCore30Demo.Entity;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.Resource;

namespace DotNetCore30Demo.IRepository
{
    public interface ISchoolRepository : IRepository<School>
    {
        Task<IEnumerable<SchoolResource>> GetAll();
    }
}
