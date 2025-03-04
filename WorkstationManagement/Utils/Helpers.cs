using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WorkstationManagement.Utils;

public class Helper 
{
    public static string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2")); 
            }
            return builder.ToString();
        }
    }

    public static (string, bool) CheckStrength(string password)
    {
        if(password.Length < 1)
            return ("Password is empty", false);

        if(password.Length < 4)
            return ("Password is short", false);

        if(!Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
            return ("Password has to have at least one number", false);

        if(!Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success)
            return ("Password has to have at least one lower case letter", false);

        if(!Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
            return ("Password has to have at least one upper case letter", false); 

        if(!Regex.Match(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
            return ("Password has to have at least one symbol", false);

        return ("", true);
    }

    //https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560
    public static bool ApproximatelyEquals(string source1, string source2, double targetRatio) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            if (source1Length == 0 || source2Length == 0)
                return false;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++){}
            for (var j = 0; j <= source2Length; matrix[0, j] = j++){}

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            double levenshteinScore = matrix[source1Length, source2Length];
            double ratio = (source1Length + source2Length - levenshteinScore)/(source1Length + source2Length );
            if(ratio >= targetRatio)
                return true;
            else 
                return false;
        }
}
