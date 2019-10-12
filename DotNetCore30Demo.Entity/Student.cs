using System;
using DotNetCore30Demo.DataAccess;

namespace DotNetCore30Demo.Entity
{
    public class Student:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime Birthday { get; set; }

        public School MySchool { get; set; }
    }
}