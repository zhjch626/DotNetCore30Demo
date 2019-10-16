using AutoMapper;
using DotNetCore30Demo.Entity;

namespace DotNetCore30Demo.Resource.AutoMapperProfile
{
    public class SchoolProfile:Profile
    {
        public SchoolProfile()
        {
            CreateMap<School, SchoolResource>();
            CreateMap<SchoolUpdateResource, School>();
        }

    }
}