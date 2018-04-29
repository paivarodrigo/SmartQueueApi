using System;
using System.Security.Cryptography;
using System.Text;

namespace Api.Utils
{
    public class Gerador
    {
        public static string HashMd5(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string NovaSenha()
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");

            Random clsRan = new Random();
            Int32 tamanhoSenha = clsRan.Next(6, 8);

            string senha = "";
            for (Int32 i = 0; i <= tamanhoSenha; i++)
                senha += guid.Substring(clsRan.Next(1, guid.Length), 1);

            return senha;
        }
    }
}
