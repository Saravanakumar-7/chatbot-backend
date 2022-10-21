using Microsoft.AspNetCore.Mvc;
using Tips.Services;

namespace Tips.Controllers
{
    public class UomController
    {
        private readonly IUomService _uomService;


        public UomController(IUomService uomService)
        {
            _uomService = uomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUom()
        {
            var uomDetails = await _uomService.GetAllUomDetails();
            return null; //Ok(uomDetails);   
        }
    }
}
