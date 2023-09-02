using AutoMapper;
using XUnitApi.DTO;
using XUnitApi.Models;

namespace XUnitApi.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Aotable, AoTableDto>();
            CreateMap<Form, FormDto>();
                }
    }
}
