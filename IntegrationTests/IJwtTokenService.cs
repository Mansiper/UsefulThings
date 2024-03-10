using Microsoft.AspNetCore.Identity;
using System.Collection.Generic;

namespace Service;

public interface IJwtTokenService
{
	string BuildToken(IdentityUser user, IList<string> roles);
}