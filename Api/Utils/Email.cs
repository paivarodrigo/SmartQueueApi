using System.Net;
using System.Net.Mail;

namespace Api.Utils
{
    public class Email
    {
        public static void EnviarNovaSenha(string email, string nome, string senha)
        {
            // Configura a mensagem e os endereços
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("smartqueueteam@gmail.com");
            mail.To.Add(email);
            mail.Subject = "SmartQueue - Nova Senha";
            mail.Body = "Olá, " + nome + "!\n\n" + "Sua nova senha para login no aplicativo é: " + senha +
                "\n\nVocê pode acessar o aplicativo com sua nova senha e alterá-la em: Configurações > Alterar Senha.\n\n" +
                "Agradecemos sua participação!\nEquipe SmartQueue.";

            // Configura SMTPClient e envia mensagem
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            // EMAIL E SENHA QUE ENVIA MENSAGEM
            smtp.Credentials = new NetworkCredential("smartqueueteam@gmail.com", "sm4rtq3u3!");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        public static void EnviarPersonalizado(string email, string assunto, string mensagem)
        {
            // Configura a mensagem e os endereços
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("smartqueueteam@gmail.com");
            mail.To.Add(email);
            mail.Subject = assunto;
            mail.Body = mensagem;

            // Configura SMTPClient e envia mensagem
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            // EMAIL E SENHA QUE ENVIA MENSAGEM
            smtp.Credentials = new NetworkCredential("smartqueueteam@gmail.com", "sm4rtq3u3!");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
