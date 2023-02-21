using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Common.DataTransferObjects.User;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        private User? _user;

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

        public Task<bool> AuthenticateUser(UserAuthenticationDto userAuthenticationDto)
        {
            if (string.IsNullOrEmpty(userAuthenticationDto.UserName))
            {
                throw new ArgumentException("Parameter UserName on UserAuthenticationDto cannot be null");
            }

            if (string.IsNullOrEmpty(userAuthenticationDto.Password))
            {
                throw new ArgumentException("Parameter Password on UserAuthenticationDto cannot be null");
            }

            return AuthenticateUserInternalAsync(userAuthenticationDto);

        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();

            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtSecret = _configuration["JwtSettings:Secret"];

            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new ArgumentException("Invalid JWT Secret");
            }

            var key = Encoding.UTF8.GetBytes(jwtSecret);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
              claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken
            (
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["Expires"])),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private async Task<bool> AuthenticateUserInternalAsync(UserAuthenticationDto userAuthenticationDto)
        {
            _user = await _userManager.FindByNameAsync(userAuthenticationDto.UserName!);

            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userAuthenticationDto.Password!));

            if (!result)
            {
                _logger.LogWarning($"Authentication failed. Wrong UserName or Password.");
            }

            return result;
        }

        private async Task<IdentityResult> RegisterUserInternalAsync(UserRegistrationDto userRegistrationDto, User user)
        {
            var result = await _userManager.CreateAsync(user, userRegistrationDto.Password!);

            if (result.Succeeded && userRegistrationDto.Roles != null)
            {
                await _userManager.AddToRolesAsync(user, userRegistrationDto.Roles);
            }

            return result;
        }
    }
}