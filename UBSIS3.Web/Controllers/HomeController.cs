using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using UBSIS3.Web.Data.Dtos;
using UBSIS3.Web.Data.Interfaces;
using UBSIS3.Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Linq;
using System;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using localization.Localization;
using MimeKit;
using MailKit.Security;
using UBSIS3.Web.Helpers;

namespace UBSIS3.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IContactUsRepository _contactUsRepo;
        private readonly ICareerRepository _careerRepo;
        private readonly IErrorLogRepository _errorRepo;
        private readonly IVacancyRepository _vacanRepo;
        private readonly IEmailNewsletterRepository _emailNewsletterRepo;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ICovRepository _covRepo;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, IContactUsRepository contactUsRepo, ICareerRepository careerRepo, IHostingEnvironment hostingEnvironment, IErrorLogRepository errorRepo, IVacancyRepository vacanRepo, IEmailNewsletterRepository emailNewsletterRepo, IStringLocalizer<HomeController> localizer, ICovRepository covRepo)
        {
            _logger = logger;
            _contactUsRepo = contactUsRepo;
            _careerRepo = careerRepo;
            _mapper = mapper;
            _errorRepo = errorRepo;
            _vacanRepo = vacanRepo;
            _hostingEnvironment = hostingEnvironment;
            _emailNewsletterRepo = emailNewsletterRepo;
            _localizer = localizer;
            _covRepo = covRepo;
        }

        [Route("/")]
        //[Route("/home")]
        //[Route("/anasayfa")]
        public IActionResult Index()
        {
            if (!_covRepo.AnyEntity())
            {
                _covRepo.CreateEntity(new Cov
                {
                    AboutUs=0,
                    Softwares=0,
                    Career=0,
                    Contact=0,
                    Home=0,
                });
            }
            ViewBag.responseCreCnt = HttpContext.Session.GetString("resultCodeCreateContact") ?? "";
            ViewBag.responseCreCar = HttpContext.Session.GetString("resultCodeCreateCareer") ?? "";
            HttpContext.Session.Remove("resultCodeCreateContact");
            HttpContext.Session.Remove("resultCodeCreateCareer");
            var cov= _covRepo.GetAllEntities().First();
            cov.Home++;
            _covRepo.UpdateEntity(cov);
            
            return View();
        }

        [Route("/Robots.txt")]
        public IActionResult Robots()
        {
            Response.ContentType = "text/plain";
            return View();
        }

        [Route("/tr/ubsis-iletisim")]
        [Route("/en/ubsis-contact")]
        [Route("/ru/ubsis-contact")]
        [HttpGet]
        public IActionResult Contact(string subject)
        {
            var cov = _covRepo.GetAllEntities().First();
            cov.Contact++;
            _covRepo.UpdateEntity(cov);
            return View(new ContactUsDto { Subject=subject});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/tr/ubsis-iletisim")]
        [Route("/en/ubsis-contact")]
        [Route("/ru/ubsis-contact")]
        public IActionResult Contact(ContactUsDto dto)
        {
            if (ModelState.IsValid)
            {
                var contactUs = _mapper.Map<ContactUs>(dto);
                bool a = _contactUsRepo.CreateEntity(contactUs);
                if (a)
                {
                    HttpContext.Session.SetString("resultCodeCreateContact", _localizer["Your message has been sent. Thank you."].ToString());

                    HttpContext.Session.SetObject("contactMessageInfo", dto);
                    EmailToAdminForContactMessage();
                    return LocalRedirect("/");
                }
                else
                {
                    return View("Contact", dto);
                }
            }
            else
            {
                return View("Contact", dto);
            }
        }

        [HttpPost]
        public JsonResult DecodeText(string imageFileName)
        {
            string myText = imageFileName.DecodeText();
            return Json(new { success = true, result = myText });
        }

        private async Task UploadResumeAsync(IFormFile fileResume, string clientName)
        {
            if (fileResume.Length > 0)
            {
                try
                {
                    string fileNam = clientName + "_resume_" + DateTime.Now.ToString("dd/MM/yyyy");
                    string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Resume");
                    string filePath = Path.Combine(folderPath, fileNam);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileResume.CopyToAsync(stream).ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    HttpContext.Session.SetString("alert", _localizer["Error occured while uploading resume. Please try again."]);
                    _errorRepo.CreateEntity(new ErrorLog
                    {
                        ErrorDetail = e.ToString(),
                        ErrorLocation= "UploadResumeAsync"
                    }); 
                }
            }
            else
            {
                HttpContext.Session.SetString("alert", _localizer["File size is 0 KB."]);
            }
        }

        public bool CheckFileExtensionsAndSizes(IFormFile fileResume)
        {
            var allowedExtensions = new[] { ".doc", ".docx", ".pdf" };
            var fileSize = fileResume.Length;
            var checkextension = Path.GetExtension(fileResume.FileName).ToLower();

            if (!allowedExtensions.Contains(checkextension))
            {
                HttpContext.Session.SetString("alert", "Only .doc, .docx and .pdf file extensions are allowed.");
                return false;
            }
            if (fileSize > 5242880)
            {
                HttpContext.Session.SetString("alert", "Max 5 MB allowed for resume file.");
                return false;
            }
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/tr/ubsis-kariyer")]
        [Route("/en/ubsis-career")]
        [Route("/ru/ubsis-career")]
        public async Task<IActionResult> Career(CareerDto dto)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            string culture = locale.RequestCulture.UICulture.ToString();

            if (ModelState.IsValid)
            {
                bool a= CheckFileExtensionsAndSizes(dto.Resume);

                if (!a)
                {
                    if (culture.Equals("tr"))
                    {
                        return LocalRedirect("/tr/ubsis-kariyer");
                    }
                    else if (culture.Equals("ru"))
                    {
                        return LocalRedirect("/ru/ubsis-career");
                    }
                    else
                    {
                        return LocalRedirect("/en/ubsis-career");
                    }
                }

                await UploadResumeAsync(dto.Resume, dto.Name);

                //if (HttpContext.Session.GetString("alert")!=null)
                //{
                //    return RedirectToAction("Career");
                //}

                var career = _mapper.Map<Career>(dto);
                career.ResumeFileName = dto.Resume.FileName;

                bool b = _careerRepo.CreateEntity(career);
                if (b)
                {
                    HttpContext.Session.SetString("resultCodeCreateCareer", _localizer["Your application has been received. Thank you for your interest."].ToString());
                    EmailToAdminForResumeWarning(dto);
                    return LocalRedirect("/");
                }
            }

            return View("Career", dto);
        }

        [Route("/tr/ubsis-kariyer")]
        [Route("/en/ubsis-career")]
        [Route("/ru/ubsis-career")]
        [HttpGet]
        public IActionResult Career()
        {
            var cov = _covRepo.GetAllEntities().First();
            cov.Career++;
            _covRepo.UpdateEntity(cov);
            ViewBag.alert = HttpContext.Session.GetString("alert") ?? "";
            HttpContext.Session.Remove("alert");
            return View(new CareerDto());
        }


        [Route("/tr/ubsis-yazilim-cozumleri")]
        [Route("/en/ubsis-software-solutions")]
        [Route("/ru/ubsis-software-solutions")]
        public IActionResult Softwares()
        {
            var cov = _covRepo.GetAllEntities().First();
            cov.Softwares++;
            _covRepo.UpdateEntity(cov);
            return View();
        }

        [Route("/tr/ubsis-hakkimizda")]
        [Route("/en/ubsis-about-us")]
        [Route("/ru/ubsis-about-us")]
        public IActionResult AboutUs()
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            ViewBag.browserCulture = locale.RequestCulture.UICulture.ToString();

            var cov = _covRepo.GetAllEntities().First();
            cov.AboutUs++;
            _covRepo.UpdateEntity(cov);
            return View();
        }

        
        [HttpPost]
        public JsonResult AddToNewsletter(string email)
        {
            if (email == null)
            {
                return Json(new { success = false, responseText = _localizer["Please enter a valid email address."].ToString() });
            }

            var emailDbcount = _emailNewsletterRepo.GetAllEntities().Count(x => x.EmailAddress.Equals(email));

            if (emailDbcount > 0)
            {
                return Json(new { success = false, responseText =_localizer["You are already registered to our email newsletter."].ToString() });
            }

            else
            {
                #region IP Kontrolü
                string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    remoteIpAddress = Request.Headers["X-Forwarded-For"];
                }
                #endregion

                EmailNewsletter newsLetter = new EmailNewsletter
                {
                    EmailAddress = email,
                    UserIp= remoteIpAddress
                };

                _emailNewsletterRepo.CreateEntity(newsLetter);
                EmailToAdminForNewsletterRecord(email);
                return Json(new { success = true, responseText = _localizer["Your subscription to the e-mail newsletter is successful. Thank you."].ToString() });
            }
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires= DateTime.Now.AddDays(1)}
                );

            string[] urlArr = returnUrl.Split("/");

            //anasayfadan geliyorsa
            if (urlArr.Length == 2 && urlArr[0].Equals("") && urlArr[1].Equals(""))
            {
                return LocalRedirect("/");
            }

            //hakkımızda sayfasından geliyorsa
            if (urlArr.Length == 3 && (urlArr[2].Equals("ubsis-hakkimizda") || urlArr[2].Equals("ubsis-about-us")))
            {
                if (culture.Equals("tr"))
                {
                    return LocalRedirect("/tr/ubsis-hakkimizda");
                }
                if (culture.Equals("ru"))
                {
                    return LocalRedirect("/ru/ubsis-about-us");
                }
                else
                {
                    return LocalRedirect("/en/ubsis-about-us");
                }
            }

            //iletişim sayfasından geliyorsa
            if (urlArr.Length == 3 && (urlArr[2].Equals("ubsis-iletisim") || urlArr[2].Equals("ubsis-contact")))
            {
                if (culture.Equals("tr"))
                {
                    return LocalRedirect("/tr/ubsis-iletisim");
                }
                if (culture.Equals("ru"))
                {
                    return LocalRedirect("/ru/ubsis-contact");
                }
                else
                {
                    return LocalRedirect("/en/ubsis-contact");
                }
            }

            //yazılım sayfasından geliyorsa
            if (urlArr.Length == 3 && (urlArr[2].Equals("ubsis-yazilim-cozumleri") || urlArr[2].Equals("ubsis-software-solutions")))
            {
                if (culture.Equals("tr"))
                {
                    return LocalRedirect("/tr/ubsis-yazilim-cozumleri");
                }
                if (culture.Equals("ru"))
                {
                    return LocalRedirect("/ru/ubsis-software-solutions");
                }
                else
                {
                    return LocalRedirect("/en/ubsis-software-solutions");
                }
            }

            //kariyer sayfasından geliyorsa
            if (urlArr.Length == 3 && (urlArr[2].Equals("ubsis-kariyer") || urlArr[2].Equals("ubsis-career")))
            {
                if (culture.Equals("tr"))
                {
                    return LocalRedirect("/tr/ubsis-kariyer");
                }
                if (culture.Equals("ru"))
                {
                    return LocalRedirect("/ru/ubsis-career");
                }
                else
                {
                    return LocalRedirect("/en/ubsis-career");
                }
            }

            return LocalRedirect("/");

        }

        public void EmailToAdminForContactMessage()
        {
            MimeMessage message = new MimeMessage();
            MimeMessage message2 = new MimeMessage();
            MimeMessage message3 = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Health Web Software",
            "ubsisprojectmanagement@gmail.com");

            message.From.Add(from);
            message2.From.Add(from);
            message3.From.Add(from);

            MailboxAddress to = new MailboxAddress("Merhaba",
            "ramazan.kizilkaya@eurasianhealth.com");
            MailboxAddress to2 = new MailboxAddress("Merhaba",
            "uysalhasan19@gmail.com");
            MailboxAddress to3 = new MailboxAddress("Merhaba",
            "kizilkayaramazan@gmail.com");

            message.To.Add(to);
            message2.To.Add(to2);
            message3.To.Add(to3);

            message.Subject = "Healthwebsoftware sitesine iletişim mesajı geldi";
            message2.Subject = "Healthwebsoftware sitesine iletişim mesajı geldi";
            message3.Subject = "Healthwebsoftware sitesine iletişim mesajı geldi";

            BodyBuilder bodyBuilder = new BodyBuilder();

            var contactMessage = HttpContext.Session.GetObject<ContactUsDto>("contactMessageInfo");
            HttpContext.Session.Remove("contactMessageInfo");

            bodyBuilder.TextBody = "Healthwebsoftware sitesine iletişim mesajı geldi. Gönderen: " + contactMessage.Name + "(" + contactMessage.Email + ")" + " Konu: " + contactMessage.Subject + " Mesaj: " + contactMessage.Message;

            message.Body = bodyBuilder.ToMessageBody();
            message2.Body = bodyBuilder.ToMessageBody();
            message3.Body = bodyBuilder.ToMessageBody();

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client2 = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client3 = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                //client.Connect("mail.misdanismanlik.com", 25, SecureSocketOptions.None);
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation="iletişim mesajı uyarı maili gönderme hatası",
                    ErrorDetail= "iletişim mesajı uyarı maili gönderme hatası"
                });
            }
            try
            {
                client2.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client2.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client2.Send(message2);
                client2.Disconnect(true);
                client2.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "iletişim mesajı uyarı maili gönderme hatası",
                    ErrorDetail = "iletişim mesajı uyarı maili gönderme hatası"
                });
            }

            try
            {
                client3.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client3.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client3.Send(message3);
                client3.Disconnect(true);
                client3.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "iletişim mesajı uyarı maili gönderme hatası",
                    ErrorDetail = "iletişim mesajı uyarı maili gönderme hatası"
                });
            }
        }

        public void EmailToAdminForResumeWarning(CareerDto dto)
        {
            MimeMessage message = new MimeMessage();
            MimeMessage message2 = new MimeMessage();
            MimeMessage message3 = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Health Web Software",
            "ubsisprojectmanagement@gmail.com");

            message.From.Add(from);
            message2.From.Add(from);
            message3.From.Add(from);

            MailboxAddress to = new MailboxAddress("Merhaba",
            "ramazan.kizilkaya@eurasianhealth.com");
            MailboxAddress to2 = new MailboxAddress("Merhaba",
            "uysalhasan19@gmail.com");
            MailboxAddress to3 = new MailboxAddress("Merhaba",
           "kizilkayaramazan@gmail.com");

            message.To.Add(to);
            message2.To.Add(to2);
            message3.To.Add(to3);

            message.Subject = "Healthwebsoftware sitesine iş başvurusu yapıldı";
            message2.Subject = "Healthwebsoftware sitesine iş başvurusu yapıldı";
            message3.Subject = "Healthwebsoftware sitesine iş başvurusu yapıldı";

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.TextBody = "Healthwebsoftware sitesine iş başvurusu yapıldı. Başvuru sahibi: " +  dto.Name + ", Eposta Adresi: " + dto.Email + ", Başvuru yapılan pozisyon: " + dto.Position + " Dosya Adı: " + dto.Resume.FileName + " Detaylar için database'i kontrol ediniz.";

            message.Body = bodyBuilder.ToMessageBody();
            message2.Body = bodyBuilder.ToMessageBody();
            message3.Body = bodyBuilder.ToMessageBody();

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client2 = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client3 = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "iş başvurusu uyarı maili gönderme hatası",
                    ErrorDetail = "iş başvurusu maili gönderme hatası"
                });
            }
            try
            {
                client2.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client2.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client2.Send(message2);
                client2.Disconnect(true);
                client2.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "iş başvurusu uyarı maili gönderme hatası",
                    ErrorDetail = "iş başvurusu uyarı maili gönderme hatası"
                });
            }
            try
            {
                client3.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client3.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client3.Send(message3);
                client3.Disconnect(true);
                client3.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "iş başvurusu uyarı maili gönderme hatası",
                    ErrorDetail = "iş başvurusu uyarı maili gönderme hatası"
                });
            }
        }

        public void EmailToAdminForNewsletterRecord(string email)
        {
            MimeMessage message = new MimeMessage();
            MimeMessage message2 = new MimeMessage();
            MimeMessage message3 = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Health Web Software",
            "ubsisprojectmanagement@gmail.com");

            message.From.Add(from);
            message2.From.Add(from);
            message3.From.Add(from);

            MailboxAddress to = new MailboxAddress("Merhaba",
            "ramazan.kizilkaya@eurasianhealth.com");
            MailboxAddress to2 = new MailboxAddress("Merhaba",
            "uysalhasan19@gmail.com");
            MailboxAddress to3 = new MailboxAddress("Merhaba",
            "kizilkayaramazan@gmail.com");

            message.To.Add(to);
            message2.To.Add(to2);
            message3.To.Add(to3);

            message.Subject = "healthwebsoftware sitesi eposta aboneliğine yeni üye kaydı yapıldı";
            message2.Subject = "healthwebsoftware sitesi eposta aboneliğine yeni üye kaydı yapıldı";
            message3.Subject = "healthwebsoftware sitesi eposta aboneliğine yeni üye kaydı yapıldı";

            BodyBuilder bodyBuilder = new BodyBuilder();

            bodyBuilder.TextBody = "Healthwebsoftware sitesi eposta bülten aboneliğine yeni üye kaydı yapıldı. Yeni kayıt yapan üyenin eposta adresi: "+ email + " Toplam eposta bülten abone sayısı: " + _emailNewsletterRepo.GetAllEntities().Count().ToString();

            message.Body = bodyBuilder.ToMessageBody();
            message2.Body = bodyBuilder.ToMessageBody();
            message3.Body = bodyBuilder.ToMessageBody();

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client2 = new MailKit.Net.Smtp.SmtpClient();
            MailKit.Net.Smtp.SmtpClient client3 = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client.Authenticate("ramazan.kizilkaya@eurasianhealth.com", "UbsisCinnah2015Eylul");
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "eposta bülten aboneliği uyarı maili gönderme hatası",
                    ErrorDetail = "eposta bülten aboneliği uyarı maili gönderme hatası"
                });
            }
            try
            {
                client2.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client2.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client2.Send(message2);
                client2.Disconnect(true);
                client2.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "eposta bülten aboneliği uyarı maili gönderme hatası",
                    ErrorDetail = "eposta bülten aboneliği uyarı maili gönderme hatası"
                });
            }
            try
            {
                //client3.Connect("mail.misdanismanlik.com", 25, SecureSocketOptions.None);
                //client.Connect("smtp.gmail.com", 587, SecureSocketOptions.);
                //client3.Authenticate("ramazan.kizilkaya@eurasianhealth.com", "UbsisCinnah2015Eylul");
                client3.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client3.Authenticate("ubsisprojectmanagement@gmail.com", "11791191");
                client3.Send(message3);
                client3.Disconnect(true);
                client3.Dispose();
            }
            catch (Exception ex)
            {
                _errorRepo.CreateEntity(new ErrorLog
                {
                    ErrorLocation = "eposta bülten aboneliği uyarı maili gönderme hatası",
                    ErrorDetail = "eposta bülten aboneliği uyarı maili gönderme hatası"
                });
            }
        }
    }
}




