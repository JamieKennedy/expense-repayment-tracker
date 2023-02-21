using Common.DataTransferObjects.User;
using Microsoft.AspNetCore.Identity;

namespace Service.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserRegistrationDto userRegistrationDto);
        Task<bool> AuthenticateUser(UserAuthenticationDto userAuthenticationDto);
        Task<string> CreateToken();
    }
}