namespace ERP.SharedKernel.Localization;

public interface ILocalizationService
{
    string GetMessage(string key, string language = "en");
}

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _messages;

    public LocalizationService()
    {
        _messages = new Dictionary<string, Dictionary<string, string>>
        {
            { "auth.invalid_credentials", new Dictionary<string, string>
                {
                    { "en", "Invalid username or password" },
                    { "ar", "«”„ «·„” Œœ„ √Ê ﬂ·„… «·„—Ê— €Ì— ’ÕÌÕ…" }
                }
            },
            { "role.notfound", new Dictionary<string, string>
                {
                    { "en", "Role not found" },
                    { "ar", "«·œÊ— €Ì— „ÊÃÊœ" }
                }
            },
            { "role.already_exists", new Dictionary<string, string>
                {
                    { "en", "Role {0} already exists" },
                    { "ar", "«·œÊ— {0} „ÊÃÊœ »«·›⁄·" }
                }
            },
            { "role.created", new Dictionary<string, string>
                {
                    { "en", "Role created successfully" },
                    { "ar", " „ ≈‰‘«¡ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.updated", new Dictionary<string, string>
                {
                    { "en", "Role updated successfully" },
                    { "ar", " „  ÕœÌÀ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.deleted", new Dictionary<string, string>
                {
                    { "en", "Role deleted successfully" },
                    { "ar", " „ Õ–› «·œÊ— »‰Ã«Õ" }
                }
            },
            { "roles.retrieved", new Dictionary<string, string>
                {
                    { "en", "Roles retrieved successfully" },
                    { "ar", " „ «” —Ã«⁄ «·√œÊ«— »‰Ã«Õ" }
                }
            },
            { "role.assigned", new Dictionary<string, string>
                {
                    { "en", "Role assigned successfully" },
                    { "ar", " „ ≈”‰«œ «·œÊ— »‰Ã«Õ" }
                }
            },
            { "role.removed", new Dictionary<string, string>
                {
                    { "en", "Role removed successfully" },
                    { "ar", " „ ≈“«·… «·œÊ— »‰Ã«Õ" }
                }
            },
            { "user.notfound", new Dictionary<string, string>
                {
                    { "en", "User not found" },
                    { "ar", "«·„” Œœ„ €Ì— „ÊÃÊœ" }
                }
            },
            { "user.email_exists", new Dictionary<string, string>
                {
                    { "en", "Email {0} is already in use" },
                    { "ar", "«·»—Ìœ «·≈·ﬂ —Ê‰Ì {0} ﬁÌœ «·«” Œœ«„ »«·›⁄·" }
                }
            },
            { "user.created", new Dictionary<string, string>
                {
                    { "en", "User created successfully" },
                    { "ar", " „ ≈‰‘«¡ «·„” Œœ„ »‰Ã«Õ" }
                }
            },
            { "user.updated", new Dictionary<string, string>
                {
                    { "en", "User updated successfully" },
                    { "ar", " „  ÕœÌÀ «·„” Œœ„ »‰Ã«Õ" }
                }
            },
            { "user.login_success", new Dictionary<string, string>
                {
                    { "en", "Login successful" },
                    { "ar", " „  ”ÃÌ· «·œŒÊ· »‰Ã«Õ" }
                }
            }
        };
    }

    public string GetMessage(string key, string language = "en")
    {
        if (string.IsNullOrWhiteSpace(language))
            language = "en";

        if (_messages.TryGetValue(key, out var translations))
        {
            if (translations.TryGetValue(language.ToLower(), out var message))
                return message;
            
            return translations.TryGetValue("en", out var englishMessage) ? englishMessage : key;
        }

        return key;
    }
}
