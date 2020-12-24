using System;
using System.IO;
using FcmbTokenUtils.Dtos;
using FcmbTokenUtils;


namespace FcmbTokenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenUtilities tu = new TokenUtilities();
            var trd = new TokenResponseDto { AccessToken = "sometoken", ExpiresIn = DateTime.Now };
            var filename = Directory.GetParent(".").Parent.Parent + "/Token/token.txt";
            Console.WriteLine(filename);

            Console.WriteLine(trd.ExpiresIn);
            Console.WriteLine(tu.TokenExists(filename));
            tu.SaveToken(filename, trd);
            Console.WriteLine(tu.TokenExists(filename));
            TokenResponseDto trd2 = tu.FetchTokenFromFile(filename);
            Console.WriteLine(trd2.AccessToken);
            Console.WriteLine(trd2.ExpiresIn);
            Console.WriteLine(trd2.RefreshToken);
        }
    }
}
