using BlogApp.DTOs;

namespace BlogApp.Services.Abstract
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<IList<CategoryDto>> GetAllCategoriesAsync();
        // Task CreateCategoryAsync(CategoryCreateDto categoryCreateDto); // Admin işlemleri için eklenebilir
        // Task UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
        // Task DeleteCategoryAsync(int id);
    }
}
