using BLL.Interfaces;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositories
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;

        public CategoryService(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAll();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepo.GetById(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepo.Add(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _categoryRepo.Update(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepo.GetById(id);
            if (category != null)
                _categoryRepo.Delete(category);
        }
    }
}
