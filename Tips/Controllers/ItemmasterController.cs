using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.Model;
using Tips.Services;

namespace Tips.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemmasterController : ControllerBase
    {
        private readonly IItemMasterServices _itemMasterService;
        public ItemmasterController(IItemMasterServices itemMasterService)
        {
            _itemMasterService = itemMasterService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var Item = _itemMasterService.GetAllItems();
            return Ok(Item);
        }

        [HttpGet("id")]
        public IActionResult GetById([FromRoute] int id)
        {
            var Item = _itemMasterService.GetItemById(id);
            if (Item == null)
            {
                return NotFound();
            }
            return Ok(Item);
        }

        [HttpPost]
        public IActionResult CreateItem([FromBody] ItemMaster itemMaster)
        {
            var Item = _itemMasterService.CreateItem(itemMaster);
            return CreatedAtAction(nameof(GetById), new { id = Item, controller = "ItemMaster" }, Item);
        }

        [HttpPut]
        public IActionResult UpdateStudent([FromBody] ItemMaster itemMaster, [FromRoute] int id)
        {
            _itemMasterService.UpdateItem(itemMaster, id);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteStudent([FromRoute] int id)
        {
            _itemMasterService.DeleteItem(id);
            return Ok();
        }
    }
}

