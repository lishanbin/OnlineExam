using AutoMapper;
using OnlineExam.Model;
using OnlineExam.Model.DTO;

namespace OnlineExam.WebApi.Utility._AutoMapper
{
    public class CustomAutoMapperProfile:Profile
    {
        public CustomAutoMapperProfile()
        {
            base.CreateMap<Student, StudentDTO>();
        }
    }
}
