using Lib.DTO;
using Lib.DTO.Category;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Categories
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _category;
        public CategoryController(IRepository<Category> category)
        {
            _category = category;
        }
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> Get()
        {
            var list = await _category.GetAllAsync();
            return Ok(list);
        }
        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int id)
        {
            return Ok(await _category.GetByIdAsync(id));
        }
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> Put(CreateCategoryDTO category)
        {
            await _category.AddAsync(new Category
            {
                CategoryName = category.CategoryName,
                Description = category.Description,
            });
            return Ok();
        }
        [HttpDelete("RemoveCategory")]
        public async Task<IActionResult> Delete(int id)
        {
            await _category.DeleteAsync(id);
            return Ok();
        }
    }
}
