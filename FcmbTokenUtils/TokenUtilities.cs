using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FcmbTokenUtils.Dtos;
using FcmbTokenUtils.Models;

namespace FcmbTokenUtils
{
    public class TokenUtilities
    {
        HttpClient client = new HttpClient();

        /// <summary>
        /// Returns a Token entity gotten from a remote resource if the
        /// token doesn't already exist in the file or if the token has expired
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>A TokenEntity Object</returns>
        public TokenResponseDto GetToken(string FileName)
        {
            FileName = Directory.GetParent(".").Parent.Parent + "/Token/token.txt";
            TokenResponseDto tokenObj = null;
            

            if (TokenExists(FileName))
            {
                tokenObj = FetchTokenFromFile(FileName);
                if (tokenObj.ExpiresIn.CompareTo(DateTime.UtcNow) <= 0 || DateTime.UtcNow.Subtract(tokenObj.ExpiresIn).TotalDays == 1)
                {
                    tokenObj = FetchToken("", new object()).Result;
                    SaveToken(FileName, tokenObj);
                }

            } else
            {
                tokenObj = FetchToken("", new object()).Result;
                SaveToken(FileName, tokenObj);
            }

           


          

            return tokenObj;
        }
        
        /// <summary>
        /// Fetches the Access Token from a remote resource
        /// </summary>
        /// <param name="Path">Path to remote resource</param>
        /// <param name="RequestDto">The RequestDto Carrying data needed to fulfill the request</param>
        /// <returns>An Asynchrounous Task</returns>
        private async Task<TokenResponseDto> FetchToken(string Path, Object RequestDto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(Path, RequestDto);
            response.EnsureSuccessStatusCode();

            TokenResponseDto content = await response.Content.ReadAsAsync<TokenResponseDto>();
            return content;
        }

        /// <summary>
        /// Saves the Token to a file
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="trd"></param>
        private void SaveToken(string FileName, TokenResponseDto trd)
        {
            Console.WriteLine("started saving");

            using (StreamWriter file =
             new StreamWriter(FileName))
               
            {
                file.Write("{0},{1},{2},{3},{4}", trd.AccessToken, trd.ExpiresIn, trd.Scope, trd.RefreshToken, trd.TokenType);
            }

            Console.WriteLine("done saving");
        }

        private bool TokenExists(string FileName)
        {
            using (StreamReader file =
             new StreamReader(FileName))

            {
                return new FileInfo(FileName).Length == 0;
            }

        }

        private TokenResponseDto FetchTokenFromFile (string FileName)
        {
            TokenResponseDto trd = null;
      
            using (StreamReader file =
            new StreamReader(FileName))

            {
                string[] line = file.ReadLine().Split();
                trd.AccessToken = line[0].Trim();
                trd.ExpiresIn = Convert.ToDateTime(long.Parse(line[1]));
                trd.Scope = line[2];
                trd.RefreshToken = line[3];
                trd.TokenType = line[4];
            }

            return trd;
        }
    }
}