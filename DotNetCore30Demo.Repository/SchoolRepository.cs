using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore30Demo.Entity;
using DotNetCore30Demo.IRepository;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.Resource;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore30Demo.Repository
{
    public class SchoolRepository: EfCoreRepository<School>, ISchoolRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SchoolRepository(DbContext context, IUnitOfWork unitOfWork,IMapper mapper) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SchoolResource>> GetAll()
        {
            var a= await _unitOfWork.QueryAsync<School>("select * from school ");
            return _mapper.Map<IEnumerable<School>, IEnumerable<SchoolResource>>(a);
        }
    }
}