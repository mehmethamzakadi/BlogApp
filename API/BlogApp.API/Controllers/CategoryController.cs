using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IReadOnlyList<Category>> Get()
        {
            return await _unitOfWork.CategoryRepository.GetAllAsync();
        }

        [HttpGet("getcategorylist")]
        public IQueryable<Category> CategoryList()
        {
            var res = _unitOfWork.CategoryRepository.GetWhere(x => x.Id == 5);
            return res;
        }

        [HttpPost]
        public async Task<Category> Post()
        {
            var result = await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = "Veritabaný" });
            await _unitOfWork.Save();
            return result;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(5);
            await _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.Save();

            return Ok(category);
        }
    }
}