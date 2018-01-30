using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace InformationSecurity.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string Login { get; set; }
		
		public string Password { get; set; }

		public string Name { get; set; }

		private string md5(string input)
		{
			byte[] buffer = new byte[input.Length];
			int i = 0;
			foreach (char c in input)
			{
				buffer[i++] = Convert.ToByte(c);
			}

			string output = "";
			byte[] hash = (new MD5CryptoServiceProvider().ComputeHash(buffer));
			foreach(byte c in hash)
			{
				output += c;
			}

			return output;
		}

		public void Check(string confirm)
		{
			if (Password != md5(confirm))
				throw new PasswordException();
			bool length = true, capital = true, digit = true, letter = true, english = false;
			if (confirm.Length >= 8)
				length = false;
			foreach(char c in confirm)
			{
				if (Convert.ToInt32(c) >= 97 && Convert.ToInt32(c) <= 122)
					letter = false;
				if (Convert.ToInt32(c) >= 48 && Convert.ToInt32(c) <= 57)
					digit = false;
				if (Convert.ToInt32(c) >= 65 && Convert.ToInt32(c) <= 90)
					capital = false;
				if (Convert.ToInt32(c) < 65 || Convert.ToInt32(c) > 122)
				{
					english = true;
					break;
				}
			}
			if (length || capital || digit || letter)
				throw new LoginException();
		}

		public void Encrypt()
		{
			Password = md5(Password);
		}
	}
}