using Entities;
using Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomRepository : IRepositoryBase<EnggBom>
    {
        Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter, SearchParames searchParams);
        Task<EnggBom> GetEnggBomById(int id);
        Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber,decimal revisionNumber);
        Task<EnggBom> GetEnggBomByFgPartNumber(string fgPartNumber);
        Task<EnggBom> GetLatestEnggBomVersionDetailByItemNumber(string fgPartNumber, decimal revisionNo);
        Task<EnggBom> UpdateEnggBomVersion(EnggBom enggBom);
        Task<IEnumerable<EnggBom>> GetAllActiveEnggBom();
        Task<IEnumerable<FGCostingSPReport>> GetFGCostingSPReportWithParam(string FGItemnumber);
        Task<int?> CreateEnggBom(EnggBom enggBom);
        Task<string> UpdateEnggBom(EnggBom enggBom);
        Task<string> DeleteEnggBom(EnggBom enggBom); 
        Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomFGItemNoListByItemNumber(string itemNumber);
        Task<IEnumerable<EnggBomDetailsDto>> GetAllEnggBomDetailsByItemNumber(string itemNumber);
        Task<IEnumerable<EnggChildBomDetailsDto>> GetAllEnggChildBomDetailsByItemNumber(string itemNumber);
         //Task<List<EnggBomFGItemNumber>> GetFgParentItems(string saItemNumber);
        Task<List<EnggBomFGItemNumber>> GetAllFgItemNumberListBySaItemNumber(string childItemNumber);
        Task <IEnumerable<object>> GetAllEnggBomItemNumberVersionList();
        Task<decimal?> GetSABomQuantity(string fgPartNumber, string saItemNumber);
        Task<EnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber,decimal revisionNumber);
        Task<IEnumerable<EngineeringBom>> GetAllEnggBomVersionListByItemNumber(string itemNumber);
        Task<IEnumerable<EnggBomItemDto>> GetAllEnggBOMItemNumber();
        Task<List<EnggBomFGItemNumberWithQtyDto>> GetFGBomItemsChildDetails(List<RfqEnggitemSourcingDto> itemNumberList);
        Task<List<EnggBomFGCostItemNumberWithQtyDto>> GetFGBomItemsChildCostingDetails(string fgItemMaster);
        Task<List<EnggBomFGItemNumberWithQtyDto>> GetSABomItemsChildDetails(string SAitemnumber, decimal SAQty);//, string SAversion);
        Task<IEnumerable<CoverageEnggChildDto>> GetEnggChildItemDetails(string ItemNumber);
        //Task<IEnumerable<EnggChildItem>> GeEnggBomChildByEnggBomId(int enggBomId);
        Task<int> GetEnggBomId(string ItemNumber);
        Task<IEnumerable<string>> GetEnggChildItemNumber(int enggBomId);
        Task<List<EnggChildItem>> GetEnggChildItemNumberByEnggbom(int bomId);
        Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomChildFGItemNoListByItemNumber(string itemNumber);
        Task<EnggBom> GetAllLatestRevAndIsReleaseEnggBom(string itemNumber);
        Task<EnggBom> GetAllLatestRevBOMIsReleaseEnggBom(string itemNumber);
        Task<FGFinalLandedandMoqPrice> GetEngganditsPP(string FGItemNumber, decimal FGRevno, List<RfqSourcingPPdetailsforEngg> rfqSourcingPPdetails);
        Task<SAFinalLandedandMoqPrice> GetEnggFGSA(string SAItemNumber, decimal SAQty, List<RfqSourcingPPdetailsforEngg> rfqSourcingPPdetails);
        Task<List<EnggChildItem>> GetChildItemsLists();
        Task<IEnumerable<EnggBomSPReport>> GetEnggBomSPReportWithParam(string? bomId);
    }
}
