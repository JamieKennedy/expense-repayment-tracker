using AutoMapper;
using Common.DataTransferObjects.User;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(ILoggerManager logger, IMapper mapper, UserManager<User> userManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        public Task<IdentityResult> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            var user = _mapper.Map<User>(userRegistrationDto);

            if (userRegistrationDto.Password == null)
            {
                throw new ArgumentException("Parameter Password on UserRegistrationDto cannot be null");
            }

            return RegisterUserInternalAsync(userRegistrationDto, user);

        }

        private async Task<IdentityResult> RegisterUserInternalAsync(UserRegistrationDto userRegistrationDto, User user)
        {
            var result = await _userManager.CreateAsync(user, userRegistrationDto.Password);

            if (result.Succeeded && userRegistrationDto.Roles != null)
            {
                await _userManager.AddToRolesAsync(user, userRegistrationDto.Roles);
            }

            return result;
        }
    }
}