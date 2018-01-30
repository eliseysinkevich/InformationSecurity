using MySql.Data.Entity;
using System.Data.Entity;

namespace InformationSecurity.Models
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class FileContext : DbContext
	{
		public FileContext(): base("connection") { }
		public DbSet<File> Files { get; set; }
		public DbSet<User> Users { get; set; }
	}
}