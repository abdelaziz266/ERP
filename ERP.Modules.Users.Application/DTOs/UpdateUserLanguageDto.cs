using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class UpdateUserLanguageDto
{
    [Required(ErrorMessage = "Language is required")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Language must be 2 characters")]
    public required string Language { get; set; }
}
