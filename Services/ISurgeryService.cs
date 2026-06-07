using practo_backend.DTOs;

namespace practo_backend.Services;

public interface ISurgeryService
{
    Task<List<SurgeryCategoryDto>> GetSurgeryCategoriesAsync();
    Task<bool> SubmitLeadAsync(SurgeryLeadCreateDto leadDto);
}
