using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileService(UserManager<ApplicationUser> manager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await manager.GetUserAsync(context.Subject);

        if (user != null)
        {
            var existingClaims = await manager.GetClaimsAsync(user);
            var claims = new List<Claim>
            {
                new Claim("username", user.UserName ?? string.Empty),
            };
            context.IssuedClaims.AddRange(claims);
            var nameClaim = existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name);
            if (nameClaim != null)
            {
                context.IssuedClaims.Add(nameClaim);
            }
        }
        
        
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}