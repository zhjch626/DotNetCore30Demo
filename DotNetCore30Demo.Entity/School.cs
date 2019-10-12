using System;
using System.Collections.Generic;
using DotNetCore30Demo.DataAccess;

namespace DotNetCore30Demo.Entity
{
    public class School:IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
    }
}
