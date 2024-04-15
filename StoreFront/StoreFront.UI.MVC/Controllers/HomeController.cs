using Microsoft.AspNetCore.Mvc;
using StoreFront.UI.MVC.Models;
using System.Diagnostics;
using MimeKit;//added for access to MimeMessage class
using MailKit.Net.Smtp;
using Microsoft.CodeAnalysis.Operations;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace StoreFront.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            //in order to complete Contact POST logic, we need credentials from appsettings.json
            //add the credentials, then return here to continue the POST logic

            //CONTACT - STEP 08
            //Build out the MimeMessage with creds 
            //Send the MimeMessage using SMTP
            if (!ModelState.IsValid)
            {
                //If the Model is not valid, return to the form and keep their info in the fields
                return View(cvm);
            }
            else
            {
                //process the sending of the email message
                string message = $"You have recieved an email from {cvm.Name} (reply to: {cvm.Email}).\n" +
                    $"*Subject: {cvm.Subject}\n" +
                    $"*Message: \n" +
                    $"{cvm.Message}";
                var mm = new MimeMessage();
                mm.From.Add(new MailboxAddress("No Reply", _config.GetValue<string>("Credentials:Email:User")));
                mm.To.Add(new MailboxAddress("Catie", _config.GetValue<string>("Credentials:Email:Recipient")));
                mm.Subject = cvm.Subject;
                mm.Body = new TextPart("HTML") { Text = message };
                mm.ReplyTo.Add(new MailboxAddress(cvm.Name, cvm.Email));

                using (var client = new SmtpClient())
                {
                    client.Connect(_config.GetValue<string>("Credentials:Email:Client"));
                    client.Authenticate(
                        _config.GetValue<string>("Credentials:Email:User"),
                        _config.GetValue<string>("Credentials:Email:Password")
                        );
                    try
                    {
                        client.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = $"There was an error sending the email. Please try again later.\nError into: {ex.StackTrace}";
                        return View(cvm);
                    }
                }

                return View("EmailConfirmation", cvm);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
