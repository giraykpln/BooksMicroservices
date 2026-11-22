using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Books.APP.Domain
{
    // Provides a factory for creating BooksDb instances at design time.
    public class BooksDbFactory : IDesignTimeDbContextFactory<BooksDb>
    {
        // The connection string in the configuration file
        // (appsettings.json).
        const string CONNECTIONSTRING = "data source=BooksDB.db";

        public BooksDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BooksDb>();
            optionsBuilder.UseSqlite(CONNECTIONSTRING);
            return new BooksDb(optionsBuilder.Options);
        }

        // Seeds the database tables with initial data.
        public void SeedDb()
        {
            var db = CreateDbContext(null);

            // Delete existing data:
            db.BookGenres.RemoveRange(db.BookGenres.ToList());
            db.Books.RemoveRange(db.Books.ToList());
            db.Authors.RemoveRange(db.Authors.ToList());
            db.Genres.RemoveRange(db.Genres.ToList());

            // Reset identity columns (for SQLite):
            db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE " +
                "SET SEQ=0 WHERE NAME='BookGenres'");
            db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE " +
                "SET SEQ=0 WHERE NAME='Books'");
            db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE " +
                "SET SEQ=0 WHERE NAME='Authors'");
            db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE " +
                "SET SEQ=0 WHERE NAME='Genres'");

            // Insert new data:
            // Genres:
            db.Genres.Add(new Genre { Name = "Novel" });
            db.Genres.Add(new Genre { Name = "Science Fiction" });
            db.Genres.Add(new Genre { Name = "Dystopia" });
            db.SaveChanges();
            // Authors with Books:
            db.Authors.Add(new Author
            {
                FirstName = "George",
                LastName = "Orwell",
                Books = new List<Book>
                {
                    new Book
                    {
                        Name = "1984",
                        NumberOfPages = 328,
                        PublishDate = new DateTime(1949, 6, 8),
                        Price = 15.5m,
                        IsTopSeller = true,
                        GenreIds = new List<int>
                        {
                            db.Genres.Single(g => g.Name == "Novel").Id,
                            db.Genres.Single(g => g.Name == "Dystopia").Id
                        }
                    },
                    new Book
                    {
                        Name = "Animal Farm",
                        NumberOfPages = 112,
                        PublishDate = new DateTime(1945, 8, 17),
                        Price = 9.75m,
                        IsTopSeller = false,
                        GenreIds = new List<int>
                        {
                            db.Genres.Single(g => g.Name == "Novel").Id
                        }
                    }
                }
            });
            db.Authors.Add(new Author
            {
                FirstName = "Isaac",
                LastName = "Asimov",
                Books = new List<Book>
                {
                    new Book
                    {
                        Name = "Foundation",
                        PublishDate = new DateTime(1951, 5, 1),
                        Price = 12m,
                        IsTopSeller = true,
                        GenreIds = new List<int>
                        {
                            db.Genres.Single(g => g.Name == "Science Fiction").Id
                        }
                    }
                }
            });
            db.Authors.Add(new Author { FirstName = "Sabahattin", LastName = "Ali" });
            db.SaveChanges();
        }
    }
}
