using System;
using FluentValidation;

namespace DotNetCore30Demo.Resource
{
    public class SchoolResource
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }
    }
}