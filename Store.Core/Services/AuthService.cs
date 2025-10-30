using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Store.Core.DTOs;
using Store.Core.DTOs.Request;
using Store.Core.DTOs.Response;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Store.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly string _environmentName;

        public AuthService(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthService> logger, string environmentName)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
            _environmentName = environmentName;
        }

        public async Task<BaseResponse<TokenResponseDTO>> Accesstoken(TokenRequestDTO requestDto)
        {
            var response = new BaseResponse<TokenResponseDTO>();

            try
            {
                SqlParameter[] parametersCheckExist = new[]
                {
                    new SqlParameter("@Email", requestDto.Email)
                };

                var user = await _userRepository.GetSingleAsync("sp_get_user_by_email", parametersCheckExist);

                if (user == null)
                {
                    response.Data = null;
                    response.Success = false;
                    response.StatusCode = 404;
                    response.Message = "Usuario no encontrado";
                    return response;
                }

                if (VerifyPassword(requestDto.Password!, user.Password))
                {

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat,
                            new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                        new Claim("idUser", user.Id.ToString()),
                        new Claim("nameUser", user.Name),
                        new Claim("emailUser", user.Email),
                        new Claim("rolUser", user.Rol.ToString())
                    };

                    var token = GenerateToken(claims);


                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Token generado.";
                    response.Data = new TokenResponseDTO()
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
                    };
                }
                else
                {
                    response.Data = null;
                    response.StatusCode = 500;
                    response.Success = false;
                    response.Message = "Usuario o contraseña incorrectos.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Token inválido.";
                response.Data = null;
                response.Errors.Add("Token", ex.ToString());

                _logger.LogError(ex.ToString());

                return response;
            }

            return response;
        }

        private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            string? jwtKey = null;

            if (_environmentName == "Development")
            {
                jwtKey = _configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("API_KEY", EnvironmentVariableTarget.User);
            }
            else
            {
                jwtKey = _configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("API_KEY");
            }

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:TokenValidityInMinutes"]!)),
                notBefore: DateTime.Now,
                signingCredentials: credentials);

            return token;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }


    }
}
