using AutoMapper;
using BlogApp.DTOs;
using BlogApp.Entities;

namespace BlogApp.Mappings
{
    public class GeneralProfile : Profile // AutoMapper.Profile sınıfından kalıtım alır
    {
        public GeneralProfile()
        {
            // --- Category Mappings ---
            CreateMap<Category, CategoryDto>().ReverseMap();
            
            CreateMap<User, UserDto>();


            CreateMap<BlogPost, BlogPostDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName ?? src.User.UserName : null)) 
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null)); 

            CreateMap<BlogPost, BlogPostDetailDto>();

            CreateMap<BlogPostCreateDto, BlogPost>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<BlogPostUpdateDto, BlogPost>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            // --- Comment Mappings ---
      
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User)); // User -> UserDto map'lemesi yukarıda tanımlı olmalı

            CreateMap<CommentCreateDto, Comment>()
                .ForMember(dest => dest.CommentDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
            
        }
    }
}
