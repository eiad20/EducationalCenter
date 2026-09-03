using AutoMapper;
using EducationalCenter.Core.Entities;
using EducationalCenter.Shared.DTOs;

namespace EducationalCenter.Web.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Existing mappings
        CreateMap<Student, StudentResponseDto>();
        CreateMap<CreateStudentRequestDto, Student>();

        CreateMap<Course, CourseResponseDto>();
        CreateMap<CreateCourseRequestDto, Course>();

        CreateMap<Class, ClassResponseDto>();
        CreateMap<CreateClassRequestDto, Class>();

        // NEW: Instructor mappings
        CreateMap<Instructor, InstructorResponseDto>();
        CreateMap<CreateInstructorRequestDto, Instructor>();

        // NEW: Payment mappings
        CreateMap<Payment, PaymentResponseDto>();
        CreateMap<CreatePaymentRequestDto, Payment>();

        // NEW: Enrollment mappings
        CreateMap<Enrollment, EnrollmentResponseDto>();
        CreateMap<CreateEnrollmentRequestDto, Enrollment>();
    }
}