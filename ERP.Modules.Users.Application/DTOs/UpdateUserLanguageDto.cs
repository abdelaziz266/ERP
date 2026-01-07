using ERP.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class UpdateUserLanguageDto
{
    [Required(ErrorMessage = "Language is required")]
    public required Language Language { get; set; }
}
