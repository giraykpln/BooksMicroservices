# BooksMicroservices

Bilkent University – CTIS 465 Microservice Development with .NET – Project Phase 1

**Team member:** Burak Giray Kaplan (22102738)

## Domain

| Entity | Properties |
|---|---|
| Book | Name, NumberOfPages, PublishDate, Price, IsTopSeller, AuthorId |
| Author | FirstName, LastName |
| Genre | Name |
| BookGenre | BookId, GenreId |

- Author 1 – many Book
- Book many – many Genre (through BookGenre)

## Projects

- **CORE:** Base classes (Record, CommandResponse, Service, DbService)
- **Books.APP:** Entities, BooksDb, MediatR requests, responses and handlers, migrations
- **Books.API:** Controllers for Books, Authors and Genres, SQLite database (BooksDB.db)
- **Books.Web:** Front-end (HTML, Bootstrap, JavaScript) for book operations

## How to Run

1. Open `BooksMS.sln` in Visual Studio 2022.
2. Right-click the solution, select **Configure Startup Projects**, choose **Multiple startup projects** and set **Books.API** and **Books.Web** to **Start**.
3. Run the solution.
   - Books.API Swagger: https://localhost:7091/swagger
   - Books.Web: https://localhost:7146
4. Send `GET /api/SeedDb` from Swagger to insert the sample data, then refresh Books.Web.
