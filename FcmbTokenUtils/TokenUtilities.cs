using System;
using System.IO;
using System.Net.Http;
using System.Xml.Serialization;
using FcmbTokenUtils.Dtos;
using FcmbTokenUtils.Models;

namespace FcmbTokenUtils
{
    public class TokenUtilities
    {
        HttpClient client = new HttpClient();

        public TokenEntity GetToken(string FileName)
        {
            FileName = Directory.GetParent(".").Parent.Parent + "/Token/token.txt";
            string token = "";

            if (TokenExists(FileName))
            {
                token = FetchTokenFromFile(FileName);
              
            } else
            {
                HttpResponseMessage response = client.PostAsJsonAsync("api/generateToken", new Object()).Result;
                response.EnsureSuccessStatusCode();

                TokenResponseDto content = response.Content.ReadAsAsync<TokenResponseDto>().Result;

                token = content.AccessToken;
                SaveToken(FileName, token);


            }

            TokenEntity tokenEntity = new TokenEntity
            {
                AccessToken = token,
                RefreshToken = "some refresh token",
                Scope = "some scope",
                ExpiresIn = DateTime.UtcNow.AddDays(4),
                TokenType = "bearee"
            };


          

            return tokenEntity;
        }

        public void SaveToken(string FileName, string token)
        {
            Console.WriteLine("started saving");

            using (StreamWriter file =
             new StreamWriter(FileName))
               
            {
                file.Write(token);
            }

            Console.WriteLine("done saving");
        }

        public bool TokenExists(string FileName)
        {
            using (StreamReader file =
             new StreamReader(FileName))

            {
                return new FileInfo(FileName).Length == 0;
            }

        }

        public string FetchTokenFromFile (string FileName)
        {
            using (StreamReader file =
            new StreamReader(FileName))

            {
                return file.ReadLine();
            }
        }
    }
}