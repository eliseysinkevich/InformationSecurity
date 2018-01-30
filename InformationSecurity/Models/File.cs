using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSecurity.Models
{
	public class File
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public int UserId { get; set; }
	}
}