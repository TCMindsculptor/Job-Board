using JobBoard.UI.MVC.Models;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace JobBoard.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel contactInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(contactInfo);
            }

            string body = string.Format(
                $"Name: {contactInfo.Name}<br />"
                + $"Email: {contactInfo.Email}<br />"
                + $"Subject: {contactInfo.Subject}<br />"
                + $"Message:<br />{contactInfo.Message}");

            MailMessage msg = new MailMessage(
                "no-reply@turnermadick.com",
                "tcmadick@gmail.com",
                contactInfo.Subject,
                body);

            msg.IsBodyHtml = true;
            msg.CC.Add("email@domain.com");

            SmtpClient client = new SmtpClient("mail.turnermadick.com");
            client.Credentials =
                new NetworkCredential("no-reply@turnermadick.com",
                "P@ssw0rd");
            client.EnableSsl = false;
            client.Port = 8889;

            using (client)
            {
                try
                {
                    client.Send(msg);
                }
                catch
                {
                    ViewBag.ErrorMessage = "There was an error sending your message.\n"
                        + "Please try again.";
                    return View(contactInfo);
                }
            }

            return View("ContactConfirmation", contactInfo);
        }
    }
}
