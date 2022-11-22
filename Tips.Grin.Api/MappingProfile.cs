using AutoMapper;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;


namespace Tips.Grin.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IQCConfirmation, GrinDto>().ReverseMap();
            CreateMap<IQCConfirmation, GrinPostDto>().ReverseMap();
            CreateMap<IQCConfirmation, GrinUpdateDto>().ReverseMap();

            CreateMap<GrinParts, GrinPartsDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsPostDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsUpdateDto>().ReverseMap();

            CreateMap<IQCConfirmation, IQCConfirmationDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationPostDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationUpdateDto>().ReverseMap();


        }
    }
}
