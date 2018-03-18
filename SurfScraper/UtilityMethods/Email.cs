using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;

namespace SurfScraper.UtilityMethods
{
    public class Email
    {
        public static void SendEmail(string subject, string message)
        {
            try
            {


                SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587);
                mySmtpClient.EnableSsl = true;
                // set smtp-client with basicAuthentication
                string fromEmail = ConfigurationManager.AppSettings["mail"];
                string fromPassword = ConfigurationManager.AppSettings["pass"];
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(fromEmail, fromPassword);
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("gs.surfscraper@gmail.com", "SurfScraper");
                MailAddress to = new MailAddress("grantmsouthwood@gmail.com", "Grant");
                MailMessage myMail = new MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = subject;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = message;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }
            catch (SmtpException e)
            {
                throw;
            }
        }
        public static void SendEmailFailure(string subject, string message)
        {
            try
            {


                SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587);
                mySmtpClient.EnableSsl = true;
                // set smtp-client with basicAuthentication
                string fromEmail = ConfigurationManager.AppSettings["mail"];
                string fromPassword = ConfigurationManager.AppSettings["pass"];
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(fromEmail, fromPassword);
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("gs.surfscraper@gmail.com", "SurfScraper");
                MailAddress to = new MailAddress("grantmsouthwood@gmail.com", "Grant");
                MailMessage myMail = new MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = subject;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = message;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;
                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);
            }
            catch (SmtpException e)
            {
                throw;
            }
        }
    }
}
