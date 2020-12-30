using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FcmbTokenUtils.Dtos;

namespace FcmbTokenUtils
{
    public class TokenUtilities
    {
        HttpClient client = new HttpClient();
        public string GenerateTokenUri { get; set; } = "";
        public string FileName { get; set; } = Directory.GetParent(".").Parent.Parent + "/Token/token.txt";


        /// <summary>
        /// Returns a Token entity gotten from a remote resource if the
        /// token doesn't already exist in the file or if the token has expired
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>A TokenEntity Object</returns>
        public TokenResponseDto GetToken()
        {
            TokenResponseDto tokenObj = null;
            

            if (TokenExists(FileName))
            {
                tokenObj = FetchTokenFromFile(FileName);
                if (tokenObj.ExpiresIn.CompareTo(DateTime.UtcNow) <= 0 || DateTime.UtcNow.Subtract(tokenObj.ExpiresIn).TotalDays == 1)
                {
                    tokenObj = FetchToken(GenerateTokenUri, new object()).Result;
                    SaveToken(FileName, tokenObj);
                }

            } else
            {
                tokenObj = FetchToken(GenerateTokenUri, new object()).Result;
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
            CreateDirectory(FileName);
            using (StreamWriter file =
             new StreamWriter(FileName))
               
            {
                file.Write("{0},{1},{2},{3},{4}", trd.AccessToken, trd.ExpiresIn, trd.Scope, trd.RefreshToken, trd.TokenType);
            }

            Console.WriteLine("done saving");
        }

        private bool TokenExists(string FileName)
        {
            if (!File.Exists(FileName)) return false;

            using (StreamReader file =
             new StreamReader(FileName))

            {
                return new FileInfo(FileName).Length > 0;
            }

        }

        private TokenResponseDto FetchTokenFromFile (string FileName)
        {
            TokenResponseDto trd = new TokenResponseDto();
      
            using (StreamReader file =
            new StreamReader(FileName))

            {
                string[] line = file.ReadLine().Split(",");
                trd.AccessToken = line[0].Trim();
                Console.WriteLine(line[1].Trim());
                trd.ExpiresIn = Convert.ToDateTime(line[1].Trim());
                trd.Scope = line[2].Trim(); ;
                trd.RefreshToken = line[3].Trim(); ;
                trd.TokenType = line[4].Trim(); ;
            }

            return trd;
        }

        private void CreateDirectory(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
        }
    }
}