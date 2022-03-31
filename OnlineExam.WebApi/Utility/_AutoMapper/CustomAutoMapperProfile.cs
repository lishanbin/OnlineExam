using AutoMapper;
using OnlineExam.Model;
using OnlineExam.Model.DTO;
using OnlineExam.WebApi.ViewModel;

namespace OnlineExam.WebApi.Utility._AutoMapper
{
    public class CustomAutoMapperProfile:Profile
    {
        public CustomAutoMapperProfile()
        {
            base.CreateMap<Student, StudentDTO>();
            base.CreateMap<Student, StudentViewModel>();
            base.CreateMap<Subject, SubjectDTO>();
            base.CreateMap<Subject,SubjectViewModel>();
            base.CreateMap<Config, ConfigDTO>();
        }
    }
}
