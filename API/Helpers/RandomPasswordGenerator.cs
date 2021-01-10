using System;
using System.Text;

namespace API.Helpers
{
    public class RandomPasswordGenerator
    {
        public static string GeneratePassword(int length)
        {
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Append(validCharacters[random.Next(validCharacters.Length)]);
            }

            return password.ToString();
        }
    }
}