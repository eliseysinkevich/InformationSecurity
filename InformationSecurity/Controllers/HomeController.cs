using InformationSecurity.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InformationSecurity.Controllers
{
	public class HomeController : Controller
	{
		// создаем контекст данных
		private FileContext db = new FileContext();
		static User current;

		[HttpGet]
		public ActionResult Index()
		{
			List<File> files = new List<File>();
			foreach(File file in db.Files)
			{
				files.Add(file);
			}
			ViewBag.Users = db.Users;
			ViewBag.Files = files;
			return View();
		}

		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(User user)
		{
			try
			{
				ReCaptchaValid(Request["g-recaptcha-response"]);
			}
			catch (ReCaptchaException)
			{
				ViewBag.Error = "Вы робот.";
				return View();
			}
			user.Encrypt();
			foreach(User existingUser in db.Users)
			{
				if (user.Login == existingUser.Login)
				{
					if (user.Password == existingUser.Password)
					{
						current = existingUser;
						return RedirectToAction("Personal");
					}
					break;
				}
			}
			ViewBag.Error = "Пара логин-пароль не верна.";
			return View();
		}

		public static void ReCaptchaValid(string response)
		{
			string secretKey = "6LdZVzwUAAAAAAgZkmDxK70wPuwWhcd6lN6xDkNc";
			WebClient client = new WebClient();
			var results = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
			JObject obj = JObject.Parse(results);
			var status = (bool)obj.SelectToken("success");
			if (!status)
				throw new ReCaptchaException();
		}

		[HttpGet]
		public ActionResult SignUp()
		{
			ViewBag.User = new User() { Name = "", Login = "" };
			return View();
		}

		[HttpPost]
		public ActionResult SignUp(User user, string confirm)
		{
			try
			{
				ReCaptchaValid(Request["g-recaptcha-response"]);
				user.Encrypt();
				user.Check(confirm);
				db.Users.Add(user);
				db.SaveChanges();
				current = user;
				return RedirectToAction("Personal");
			}
			catch (ReCaptchaException)
			{
				ViewBag.Error = new string[] { "Вы робот." };
			}
			catch (LoginException)
			{
				ViewBag.Error = new string[] { "Длина пароля должна быть не менее 8 символов,", "в нем должны присутствовать латинские буквы,", "арабские цифры и хотя бы одна заглавная латинская буква." };
			}
			catch (System.Data.Entity.Infrastructure.DbUpdateException)
			{
				ViewBag.Error = new string[] { "Пользователь с таким логином уже зарегестрирован." };
			}
			catch (PasswordException)
			{
				ViewBag.Error = new string[] { "Введенные пароли не совпадают." };
			}
			ViewBag.Name = user.Name;
			ViewBag.Login = user.Login;
			return View();
		}

		public ActionResult Personal()
		{
			ViewBag.User = current;
			List<File> files = new List<File>();
			foreach (File file in db.Files)
			{
				if(file.UserId == current.Id)
					files.Add(file);
			}
			ViewBag.Files = files;
			return View();
		}

		public ActionResult SignOut()
		{
			current = null;
			ViewBag.User = null;
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Upload()
		{
			ViewBag.User = current;
			return View();
		}

		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase upload)
		{
			if (upload != null)
			{
				string fileName = System.IO.Path.GetFileName(upload.FileName);

				//Генерация случайной строки
				Random random = new Random();
				string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
				int length = 20;
				StringBuilder builder = new StringBuilder(length);
				for (int i = 0; i < length; i++)
					builder.Append(chars[random.Next(chars.Length)]);
				string path = builder.ToString();

				File file = new File() { Path = path, UserId = current.Id, Name = fileName };
				db.Files.Add(file);
				db.SaveChanges();

				upload.SaveAs(Server.MapPath("~/Files/" + path));
			}
			return RedirectToAction("Personal");
		}

		[HttpGet]
		public ActionResult ChangePassword()
		{
			ViewBag.User = current;
			return View();
		}

		[HttpPost]
		public ActionResult ChangePassword(string oldPassword, string password, string confirm)
		{
			User user = new User() { Password = oldPassword };
			user.Encrypt();
			User existingUser = db.Users.Find(current.Id);
			if (user.Password == existingUser.Password)
			{
				try
				{
					existingUser.Password = password;
					existingUser.Encrypt();
					existingUser.Check(confirm);
					db.SaveChanges();
					return RedirectToAction("Personal");
				}
				catch (PasswordException)
				{
					ViewBag.Error = "Введенные пароли не совпадают.";
				}
			}
			else
			{
				ViewBag.Error = "Введен неправильный пароль.";
			}
			return View();
		}
	}
}