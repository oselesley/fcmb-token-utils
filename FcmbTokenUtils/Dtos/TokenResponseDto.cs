﻿using System;
namespace FcmbTokenUtils.Dtos
{
    public class TokenResponseDto
    {
        public string Scope { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public TokenResponseDto()
        {
        }
    }
}
