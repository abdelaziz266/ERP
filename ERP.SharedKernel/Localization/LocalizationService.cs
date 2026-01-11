using ERP.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Resources;

namespace ERP.SharedKernel.Localization;

public class LocalizationService: ILocalizationService
{
    private readonly ResourceManager _resourceManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _resourceManager = new ResourceManager(
            "ERP.SharedKernel.Localization.Resources.Messages",
            typeof(LocalizationService).Assembly
        );
        
        _httpContextAccessor = httpContextAccessor;
    }
    public string Get(string key, params object[] args)
    {
        var culture = CurrentLanguage == Language.ar
            ? new CultureInfo("ar")
            : new CultureInfo("en");

        var value = _resourceManager.GetString(key, culture) ?? key;

        return args.Length > 0
            ? string.Format(value, args)
            : value;
    }

    private Language CurrentLanguage
    {
        get
        {
            var langClaim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("Language")?.Value;

            return Enum.TryParse<Language>(langClaim, out var lang)
                ? lang
                : Language.en;
        }
    }

}
