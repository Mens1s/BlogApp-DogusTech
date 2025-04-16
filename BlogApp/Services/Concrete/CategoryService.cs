using AutoMapper;
using BlogApp.Data.Abstract;
using BlogApp.DTOs;
using BlogApp.Services.Abstract;

namespace BlogApp.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IList<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync(x => x.Id > 0, x => x.Id);
                return _mapper.Map<IList<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories.");
                return new List<CategoryDto>();
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null) return null;
                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category with Id {CategoryId}", id);
                return null;
            }
        }

        // Create, Update, Delete metotları da benzer şekilde implemente edilebilir - zaman yok ödev vize final puff
    }
}
