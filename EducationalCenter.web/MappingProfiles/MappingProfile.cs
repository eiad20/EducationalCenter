using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Shared.DTOs;

namespace EducationalCenter.Web.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentResponseDto>();
        CreateMap<CreateStudentRequestDto, Student>();

        CreateMap<Course, CourseResponseDto>();
        CreateMap<CreateCourseRequestDto, Course>();

        CreateMap<Class, ClassResponseDto>();
        CreateMap<CreateClassRequestDto, Class>();
    }
}