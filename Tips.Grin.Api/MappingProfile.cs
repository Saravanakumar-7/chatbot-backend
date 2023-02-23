using AutoMapper;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;


namespace Tips.Grin.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Grins, GrinDto>().ReverseMap();
            CreateMap<Grins, GrinPostDto>().ReverseMap();
            CreateMap<Grins, GrinUpdateDto>().ReverseMap();
            CreateMap<Grins, GrinNumberListDto>().ReverseMap();

            CreateMap<GrinParts, GrinPartsDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsPostDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsUpdateDto>().ReverseMap();


            CreateMap<ProjectNumbers, ProjectNumbersDto>().ReverseMap();
            CreateMap<ProjectNumbers, ProjectNumbersDtoPost>().ReverseMap();
            CreateMap<ProjectNumbers, ProjectNumbersDtoUpdate>().ReverseMap();

            CreateMap<IQCConfirmation, IQCConfirmationDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationPostDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationUpdateDto>().ReverseMap();

            CreateMap<IQCConfirmationItems, IQCConfirmationItemsDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, IQCConfirmationItemsPostDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, IQCConfirmationItemsUpdateDto>().ReverseMap();

            CreateMap<Binning, BinningDto>().ReverseMap();
            CreateMap<Binning, BinningPostDto>().ReverseMap();
            CreateMap<Binning, BinningUpdateDto>().ReverseMap();

            CreateMap<BinningLocation, BinningLocationDto>().ReverseMap();
            CreateMap<BinningLocation, BinningLocationPostDto>().ReverseMap();
            CreateMap<BinningLocation, BinningLocationUpdateDto>().ReverseMap();

            CreateMap<BinningItems, BinningItemsDto>().ReverseMap();
            CreateMap<BinningItems, BinningItemsPostDto>().ReverseMap();
            CreateMap<BinningItems, BinningItemsUpdateDto>().ReverseMap();

            CreateMap<DocumentUpload, DocumentUploadDto>().ReverseMap();
            CreateMap<DocumentUpload, DocumentUploadPostDto>().ReverseMap();
            CreateMap<DocumentUpload, DocumentUploadUpdateDto>().ReverseMap();

            CreateMap<ReturnGrin, ReturnGrinDto>().ReverseMap();
            CreateMap<ReturnGrin, ReturnGrinDtoPost>().ReverseMap();

            CreateMap<ReturnGrinParts, ReturnGrinPartsDto>().ReverseMap();
            CreateMap<ReturnGrinParts, ReturnGrinPartsDtoPost>().ReverseMap();
            CreateMap<ReturnGrinParts, ReturnGrinPartsListDto>().ReverseMap();
        

            CreateMap<ReturnGrinDocumentUpload, ReturnGrinDocumentUploadDto>().ReverseMap();
            CreateMap<ReturnGrinDocumentUpload, ReturnGrinDocumentUploadDtoPost>().ReverseMap();

            CreateMap<GrinPartsPostDto, GrinUpdateQtyDetailsDto>().ReverseMap();




        }
    }
}
