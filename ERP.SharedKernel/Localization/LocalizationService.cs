using ERP.SharedKernel.Enums;

namespace ERP.SharedKernel.Localization;

public interface ILocalizationService
{
    string GetMessage(string key, Language language = Language.en);
}

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<Language, string>> _messages;

    public LocalizationService()
    {
        _messages = new Dictionary<string, Dictionary<Language, string>>
        {
            { "auth.invalid_credentials", new Dictionary<Language, string>
                {
                    { Language.en, "Invalid username or password" },
                    { Language.ar, "«”„ «·„” Œœ„ «Ê ﬂ·„… «·„—Ê— €Ì— ’ÕÌÕ…" }
                }
            },
            { "role.notfound", new Dictionary<Language, string>
                {
                    { Language.en, "Role not found" },
                    { Language.ar, "«·œÊ— €Ì— „ÊÃÊœ" }
                }
            },
            { "role.already_exists", new Dictionary<Language, string>
                {
                    { Language.en, "Role {0} already exists" },
                    { Language.ar, "«·œÊ— {0} „ÊÃÊœ »«·›⁄·" }
                }
            },
            { "role.created", new Dictionary<Language, string>
                {
                    { Language.en, "Role created successfully" },
                    { Language.ar, " „ ≈‰‘«¡ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.updated", new Dictionary<Language, string>
                {
                    { Language.en, "Role updated successfully" },
                    { Language.ar, " „  ÕœÌÀ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.deleted", new Dictionary<Language, string>
                {
                    { Language.en, "Role deleted successfully" },
                    { Language.ar, " „ Õ–› «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.has_users", new Dictionary<Language, string>
                {
                    { Language.en, "Cannot delete role because it has assigned users" },
                    { Language.ar, "·« Ì„ﬂ‰ Õ–› «·œÊ— ·√‰Â „— »ÿ »„” Œœ„Ì‰" }
                }
            },
            { "roles.retrieved", new Dictionary<Language, string>
                {
                    { Language.en, "Roles retrieved successfully" },
                    { Language.ar, " „ «” —Ã«⁄ «·√œÊ«— »‰Ã«Õ" }
                }
            },
            { "role.assigned", new Dictionary<Language, string>
                {
                    { Language.en, "Role assigned successfully" },
                    { Language.ar, " „  ⁄ÌÌ‰ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.removed", new Dictionary<Language, string>
                {
                    { Language.en, "Role removed successfully" },
                    { Language.ar, " „ ≈“«·… «·œÊ— »‰Ã«Õ" }
                }
            },
            { "user.notfound", new Dictionary<Language, string>
                {
                    { Language.en, "User not found" },
                    { Language.ar, "«·„” Œœ„ €Ì— „ÊÃÊœ" }
                }
            },
            { "user.email_exists", new Dictionary<Language, string>
                {
                    { Language.en, "Email {0} is already in use" },
                    { Language.ar, "«·»—Ìœ «·≈·ﬂ —Ê‰Ì {0} „” Œœ„ »«·›⁄·" }
                }
            },
            { "user.userName_exists", new Dictionary<Language, string>
                {
                    { Language.en, "Username {0} is already in use" },
                    { Language.ar, "«”„ «·„” Œœ„ {0} „” Œœ„ »«·›⁄·" }
                }
            },
            { "user.created", new Dictionary<Language, string>
                {
                    { Language.en, "User created successfully" },
                    { Language.ar, " „ ≈‰‘«¡ «·„” Œœ„ »‰Ã«Õ" }
                }
            },
            { "user.updated", new Dictionary<Language, string>
                {
                    { Language.en, "User updated successfully" },
                    { Language.ar, " „  ÕœÌÀ «·„” Œœ„ »‰Ã«Õ" }
                }
            },
            { "user.deleted", new Dictionary<Language, string>
                {
                    { Language.en, "User deleted successfully" },
                    { Language.ar, " „ Õ–› «·„” Œœ„ »‰Ã«Õ" }
                }
            },
            { "users.retrieved", new Dictionary<Language, string>
                {
                    { Language.en, "Users retrieved successfully" },
                    { Language.ar, " „ «” —Ã«⁄ «·„” Œœ„Ì‰ »‰Ã«Õ" }
                }
            },
            { "user.login_success", new Dictionary<Language, string>
                {
                    { Language.en, "Login successful" },
                    { Language.ar, " „  ”ÃÌ· «·œŒÊ· »‰Ã«Õ" }
                }
            }
        };
    }

    public string GetMessage(string key, Language language = Language.en)
    {
        if (_messages.TryGetValue(key, out var translations))
        {
            if (translations.TryGetValue(language, out var message))
                return message;
            
            return translations.TryGetValue(Language.en, out var englishMessage) ? englishMessage : key;
        }

        return key;
    }
}
