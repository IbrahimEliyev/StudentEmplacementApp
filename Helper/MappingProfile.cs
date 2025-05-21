using AutoMapper;
using StudentEmplacementApp.DTOs;
using StudentEmplacementApp.Models;

namespace StudentEmplacementApp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Student, StudentDto>() // for get methods
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")) // Map Name
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.SecondaryScore, opt => opt.MapFrom(src => src.SecondaryScore))
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.StudentChoices.Select(sc => sc.Choice)))
                .ForMember(dest => dest.ResultCode, opt => opt.MapFrom(src => src.ResultCode));
    

            CreateMap<Choice , ChoiceDto>()
                .ForMember(dest => dest.Code , opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.NumOfPlaces , opt => opt.MapFrom(src => src.NumOfPlaces))
                .ForMember(dest => dest.EnterenceScore, opt => opt.MapFrom(src => src.EnterenceScore))
                .ForMember(dest => dest.Major, opt => opt.MapFrom(src => new MajorDto
                {
                    Name = src.Major.Name,
                    Language = src.Major.Language.ToString(), // Convert enum to string
                    IsPaid = src.Major.IsPaid ? "Paid" : "Not Paid"
                }))
                .ForMember(dest => dest.University, opt => opt.MapFrom(src => new UniversityDto
                {
                    Name = src.University.FullName + "(" + src.University.ShortName + ")"
                }));


            CreateMap<Major, MajorDto>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language.ToString()))
                 .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid ? "Paid" : "Not Paid"));


            CreateMap<University, UniversityDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName + "(" + src.ShortName + ")"));

            // Show Admin as AdminDto within Get methods
            CreateMap<User, AdminDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
                
            // For registering admin
            CreateMap<AdminRegisterDto, User>()
                .ForMember(dest => dest.UserName, opt=> opt.MapFrom(src => src.Pin))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
        }
    }
    
}