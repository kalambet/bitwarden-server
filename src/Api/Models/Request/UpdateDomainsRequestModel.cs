using System.Text.Json;
using Bit.Core.Entities;

namespace Bit.Api.Models.Request;

public class UpdateDomainsRequestModel
{
    public IEnumerable<IEnumerable<string>> EquivalentDomains { get; set; }
    public IEnumerable<int> ExcludedGlobalEquivalentDomains { get; set; }

    public User ToUser(User existingUser)
    {
        existingUser.EquivalentDomains = EquivalentDomains != null ? JsonSerializer.Serialize(EquivalentDomains) : null;
        existingUser.ExcludedGlobalEquivalentDomains = ExcludedGlobalEquivalentDomains != null ?
            JsonSerializer.Serialize(ExcludedGlobalEquivalentDomains) : null;
        return existingUser;
    }
}
