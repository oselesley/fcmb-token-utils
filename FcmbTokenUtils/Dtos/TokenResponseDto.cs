using System;
namespace FcmbTokenUtils.Dtos
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public TokenResponseDto()
        {
        }
    }
}
