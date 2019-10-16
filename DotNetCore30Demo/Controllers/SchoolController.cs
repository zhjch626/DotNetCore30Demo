using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.Entity;
using DotNetCore30Demo.IRepository;
using DotNetCore30Demo.Resource;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCore30Demo.Controllers
{
    /// <summary>
    /// 校管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        public SchoolController(ISchoolRepository schoolRepository, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _schoolRepository = schoolRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]//swagger隐藏接口
        public async Task<ActionResult<bool>> Get(string id)
        {
            await _schoolRepository.InsertAsync(new List<School>()
            {
                new School()
                {
                    Id = new Guid(),
                    Name = "测试学校1"
                },
                new School()
                {
                    Id = new Guid(),
                    Name = "测试学校2"
                },
                new School()
                {
                    Id = new Guid(),
                    Name = "测试学校3"
                },
                new School()
                {
                    Id = new Guid(),
                    Name = "测试学校4"
                }
            });
            return await _unitOfWork.SaveChangesAsync() > 0;

        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<bool>> Insert([FromBody]SchoolResource resource)
        {
            await _schoolRepository.InsertAsync(new School()
            {
                Id = new Guid(),
                Name = resource.Name
            });
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SchoolResource>> GetSchoolAll()
        {
            var list = await _schoolRepository.GetAll();

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,                                   //格式化json字符串
                AllowTrailingCommas = true,                             //可以结尾有逗号
                //IgnoreNullValues = true,                              //可以有空值,转换json去除空值属性
                IgnoreReadOnlyProperties = true,                        //忽略只读属性
                PropertyNameCaseInsensitive = true,                     //忽略大小写
                //PropertyNamingPolicy = JsonNamingPolicy.CamelCase     //命名方式是默认还是CamelCase
            };
            //对象转为json 字符串
            //测试.net 3.0 内置的json
            string json = JsonSerializer.Serialize(list,options);

            var result = JsonSerializer.Deserialize<IList<SchoolResource>>(json);

            // var result= _mapper.Map<IList<School>,IList<SchoolResource>>(list.ToList());
            return list;
        }
    }
}