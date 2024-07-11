using AutoMapper;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;


namespace Tips.Grin.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           
            CreateMap<Grins, GrinIQCConfirmationSaveDto>().ReverseMap();
            CreateMap<GrinParts, GrinIQCConfirmationItemsSaveDto>().ReverseMap();

            CreateMap<IQCConfirmation, GrinIQCConfirmationSaveDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, GrinIQCConfirmationItemsSaveDto>().ReverseMap();

            CreateMap<ProjectNumbersDtoPost, GrinUpdateProjectBalQtyDetailsDto>().ReverseMap();

            CreateMap<GrinsForServiceItems, GrinsForServiceItemsDto>().ReverseMap();
            CreateMap<GrinsForServiceItems, GrinsForServiceItemsPostDto>().ReverseMap();
            CreateMap<GrinsForServiceItems, GrinsForServiceItemsUpdateDto>().ReverseMap();

            CreateMap<GrinsForServiceItemsParts, GrinsForServiceItemsPartsDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsParts, GrinsForServiceItemsPartsPostDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsParts, GrinsForServiceItemsPartsUpdateDto>().ReverseMap();

            CreateMap<GrinsForServiceItemsProjectNumbers, GrinsForServiceItemsProjectNumbersDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsProjectNumbers, GrinsForServiceItemsProjectNumbersPostDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsProjectNumbers, GrinsForServiceItemsProjectNumbersUpdateDto>().ReverseMap();

            CreateMap<GrinsForServiceItemsOtherCharges, GrinsForServiceItemsOtherChargesDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsOtherCharges, GrinsForServiceItemsOtherChargesPostDto>().ReverseMap();
            CreateMap<GrinsForServiceItemsOtherCharges, GrinsForServiceItemsOtherChargesUpdateDto>().ReverseMap();

            CreateMap<Grins, GrinDto>().ReverseMap();
            CreateMap<Grins, GrinPostDto>().ReverseMap();
            CreateMap<Grins, GrinUpdateDto>().ReverseMap();
            CreateMap<Grins, GrinNumberListDto>().ReverseMap();
            CreateMap<Grins, GrinItemMasterEnggDto>().ReverseMap();
            
            CreateMap<GrinParts, GrinPartsDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsPostDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsUpdateDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartsItemMasterEnggDto>().ReverseMap();
            CreateMap<GrinParts, GrinPartscalculationofAvgcost>().ReverseMap();
            CreateMap<GrinPartsPostDto, GrinPartscalculationofAvgcost>().ReverseMap();
            CreateMap<GrinPartsUpdateDto, GrinPartscalculationofAvgcost>().ReverseMap();
            
            CreateMap<ProjectNumbers, ProjectNumbersDto>().ReverseMap();
            CreateMap<ProjectNumbers, ProjectNumbersDtoPost>().ReverseMap();
            CreateMap<ProjectNumbers, ProjectNumbersDtoUpdate>().ReverseMap();
            CreateMap<ProjectNumbers, ProjectNumberscalculationofAvgcostDto>().ReverseMap();
            CreateMap<ProjectNumbersDtoPost, ProjectNumberscalculationofAvgcostDto>().ReverseMap();
            CreateMap<ProjectNumbersDtoUpdate, ProjectNumberscalculationofAvgcostDto>().ReverseMap();
            
            CreateMap<IQCConfirmation, IQCConfirmationDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationPostDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationUpdateDto>().ReverseMap();
            CreateMap<IQCConfirmation, IQCConfirmationSaveDto>().ReverseMap();

            CreateMap<IQCForServiceItems, IQCForServiceItemsDto>().ReverseMap();
            CreateMap<IQCForServiceItems, IQCForServiceItemsPostDto>().ReverseMap();
            CreateMap<IQCForServiceItems, IQCForServiceItemsSaveDto>().ReverseMap();

            CreateMap<IQCForServiceItems_Items, IQCForServiceItems_ItemsDto>().ReverseMap();
            CreateMap<IQCForServiceItems_Items, IQCForServiceItems_ItemsPostDto>().ReverseMap();
            CreateMap<IQCForServiceItems_Items, IQCForServiceItems_ItemsSaveDto>().ReverseMap();

            CreateMap<IQCConfirmationItems, IQCConfirmationItemsDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, IQCConfirmationItemsPostDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, IQCConfirmationItemsUpdateDto>().ReverseMap();
            CreateMap<IQCConfirmationItems, IQCConfirmationItemsSaveDto>().ReverseMap();

            CreateMap<Binning, BinningDto>().ReverseMap();
            CreateMap<Binning, BinningPostDto>().ReverseMap();
            CreateMap<Binning, BinningUpdateDto>().ReverseMap();
            CreateMap<Binning, BinningSaveDto>().ReverseMap();

            CreateMap<BinningLocation, BinningLocationDto>().ReverseMap();
            CreateMap<BinningLocation, BinningLocationPostDto>().ReverseMap();
            CreateMap<BinningLocation, BinningLocationUpdateDto>().ReverseMap();
            CreateMap<BinningLocation, BinningLocationSaveDto>().ReverseMap();

            CreateMap<BinningItems, BinningItemsDto>().ReverseMap();
            CreateMap<BinningItems, BinningItemsPostDto>().ReverseMap();
            CreateMap<BinningItems, BinningItemsUpdateDto>().ReverseMap();
            CreateMap<BinningItems, BinningItemsSaveDto>().ReverseMap();

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
            CreateMap<GrinPartsPostDto, GrinQtyPoStatusUpdateDto>().ReverseMap();

            CreateMap<Grins, IQCConfirmationDto>().ReverseMap();
            CreateMap<GrinParts, IQCConfirmationItemsDto>().ReverseMap();

            CreateMap<Grins, BinningDto>().ReverseMap();
            CreateMap<GrinParts, BinningItemsDto>().ReverseMap();
            CreateMap<GrinParts, BinningLocationDto>().ReverseMap();

            CreateMap<OpenGrin, OpenGrinDto>().ReverseMap();
            CreateMap<OpenGrin, OpenGrinPostDto>().ReverseMap();
            CreateMap<OpenGrin, OpenGrinUpdateDto>().ReverseMap();

            CreateMap<OpenGrinParts, OpenGrinPartsDto>().ReverseMap();
            CreateMap<OpenGrinParts, OpenGrinPartsPostDto>().ReverseMap();
            CreateMap<OpenGrinParts, OpenGrinPartsUpdateDto>().ReverseMap();

            CreateMap<OtherCharges, OtherChargesDto>().ReverseMap();
            CreateMap<OtherCharges, OtherChargesPostDto>().ReverseMap();
            CreateMap<OtherCharges, OtherChargesUpdateDto>().ReverseMap();

            CreateMap<OpenGrinDetails, OpenGrinDetailsDto>().ReverseMap();
            CreateMap<OpenGrinDetails, OpenGrinDetailsPostDto>().ReverseMap();
            CreateMap<OpenGrinDetails, OpenGrinDetailsUpdateDto>().ReverseMap();
        }
    }
}
