using Store.Core.DTOs.Request;
using Store.Core.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<TokenResponseDTO>> Accesstoken(TokenRequestDTO requestDto);
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hashedPassword);
    }
}
