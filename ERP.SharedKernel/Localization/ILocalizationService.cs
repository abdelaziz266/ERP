using ERP.SharedKernel.Enums;

namespace ERP.SharedKernel.Localization;

public interface ILocalizationService
{
    string Get (string key, params object[] args);
}
