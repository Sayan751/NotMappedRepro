using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace NotMappedRepro
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var options = new DbContextOptionsBuilder<FooContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			await using var context = new FooContext(options);
			var tokens = await context.Foos.ToListAsync();
			Console.WriteLine(JsonSerializer.Serialize(tokens));
		}
	}

	public class Foo
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public string SomeProp { get; set; }
	}

	public class Bar
	{
		[Key]
		public int Id { get; set; }
		
		[Required]
		public int FooId { get; set; }

		[ForeignKey(nameof(FooId)), IgnoreDataMember]
		public virtual Foo SomeProp { get; set; }
	}

	class FooContext : DbContext
	{
		public DbSet<Foo> Foos { get; set; }
		public DbSet<Bar> Bars { get; set; }

		public FooContext(DbContextOptions options): base(options)
		{
		}
	}
}
