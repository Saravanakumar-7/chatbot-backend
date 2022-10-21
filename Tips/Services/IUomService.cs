using Tips.Model;

namespace Tips.Services
{
    public interface IUomService
    {
        public Task<List<Uom>> GetAllUomDetails();
        public Task<List<Uom>> GetActiveUomDetails();
        public Task<Uom> GetUomDetailById(int id);
        public Task<Uom> CreateUom(Uom uom);
        public Task<Uom> UpdateUomDetails(int id,Uom uom);
        public Task<string> ActivateUom(int id);
        public Task<string> DeactivateUom(int id);
    }
}
