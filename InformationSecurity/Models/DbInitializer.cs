using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace InformationSecurity.Models
{
	public class DbInitializer : DropCreateDatabaseIfModelChanges<FileContext>
	{
		protected override void Seed(FileContext db)
		{
			using (MySqlConnection connection=new MySqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString))
			{
				connection.Open();
				MySqlCommand command = new MySqlCommand("ALTER TABLE `inf_sec`.`users` ADD UNIQUE `uq_login` (`Login`(50));", connection);
				command.ExecuteNonQuery();
			}
		}
	}
}