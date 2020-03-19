using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore30Demo.DataAccess;
using DotNetCore30Demo.Entity;
using DotNetCore30Demo.IRepository;
using DotNetCore30Demo.Resource;
using DotNetCore30Demo.Utility.Helper;
using Microsoft.AspNetCore.Authorization;
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

        public SchoolController(ISchoolRepository schoolRepository, IUnitOfWork unitOfWork, IMapper mapper) => (_schoolRepository, _unitOfWork, _mapper) = (schoolRepository, unitOfWork, mapper);

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]//swagger隐藏接口
        public async Task<ActionResult<SchoolResource>> GetById(string id)
        {
            var result = await _schoolRepository.GetByIdAsync(id);

            return _mapper.Map<School, SchoolResource>(result);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<bool>> Add([FromBody]SchoolAddResource resource)
        {
            await _schoolRepository.InsertAsync(new School()
            {
                Id = new Guid(),
                Name = resource.Name
            });
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> Update([FromBody]SchoolUpdateResource resource)
        {
            _schoolRepository.Update(_mapper.Map<SchoolUpdateResource, School>(resource));
            return _unitOfWork.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
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
            string json = JsonSerializer.Serialize(list, options);

            var result = JsonSerializer.Deserialize<IList<SchoolResource>>(json);

            //var result= _mapper.Map<IList<School>,IList<SchoolResource>>(list.ToList());
            return list;
        }

        /// <summary>
        /// 测试Aes加密解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> TesAes(string str)
        {
            string purpose = "这个算法是用来搞SSO的";
            // 返回：AcfCe3AQcmNkeNThv-u09H_HyGKy_iRy-7uGiW0IZOHI
            var aseString = AesHelper.Encrypt("密码here", purpose, Encoding.UTF8.GetBytes(str));
            // 返回：str
            return Encoding.UTF8.GetString(AesHelper.Decrypt(aseString, "密码here", purpose));
        }
    }
}