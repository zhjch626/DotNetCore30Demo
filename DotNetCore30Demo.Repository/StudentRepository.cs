using DotNetCore30Demo.Entity;
using DotNetCore30Demo.IRepository;
using DotNetCore30Demo.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore30Demo.Repository
{
    public class StudentRepository : EfCoreRepository<Student>, IStudentRepository
    {
        public StudentRepository(DbContext context) : base(context)
        {
        }
    }
}