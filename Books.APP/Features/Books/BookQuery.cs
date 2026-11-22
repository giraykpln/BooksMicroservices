using CORE.Domain;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;
using Books.APP.Features.Authors;
using Books.APP.Features.Genres;

namespace Books.APP.Features.Books
{
    public class BookQueryRequest : Record, IRequest<IQueryable<BookQueryResponse>>
    {
    }

    public class BookQueryResponse : Record
    {
        public string Name { get; set; }

        public short? NumberOfPages { get; set; }

        public DateTime PublishDate { get; set; }

        public decimal Price { get; set; }

        public bool IsTopSeller { get; set; }

        public int AuthorId { get; set; }

        public List<int> GenreIds { get; set; }

        public string NumberOfPagesR { get; set; }

        public string PublishDateR { get; set; }

        public string PriceR { get; set; }

        public string IsTopSellerR { get; set; }

        public string AuthorNameR { get; set; }

        public AuthorQueryResponse AuthorR { get; set; }

        public string GenreNamesR { get; set; }

        public List<GenreQueryResponse> GenresR { get; set; }
    }

    public class BookQueryHandler : DbService<Book>,
        IRequestHandler<BookQueryRequest, IQueryable<BookQueryResponse>>
    {
        public BookQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbQuery()
        {
            return base.DbQuery().Include(b => b.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .OrderByDescending(b => b.IsTopSeller)
                .ThenByDescending(b => b.PublishDate).ThenBy(b => b.Name);
        }

        public Task<IQueryable<BookQueryResponse>> Handle(BookQueryRequest request, CancellationToken cancellationToken)
        {
            var query = DbQuery().Select(b => new BookQueryResponse
            {
                AuthorId = b.AuthorId,
                GenreIds = b.GenreIds,
                Id = b.Id,
                IsTopSeller = b.IsTopSeller,
                Name = b.Name,
                NumberOfPages = b.NumberOfPages,
                Price = b.Price,
                PublishDate = b.PublishDate,

                NumberOfPagesR = b.NumberOfPages.HasValue ? b.NumberOfPages.Value.ToString()
                    : string.Empty,
                PublishDateR = b.PublishDate.ToShortDateString(),
                PriceR = b.Price.ToString("C2"),
                IsTopSellerR = b.IsTopSeller ? "Yes" : "No",
                AuthorNameR = b.Author.FirstName + " " + b.Author.LastName,
                AuthorR = new AuthorQueryResponse
                {
                    Id = b.Author.Id,
                    FirstName = b.Author.FirstName,
                    LastName = b.Author.LastName
                },
                GenreNamesR = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name)),
                GenresR = b.BookGenres.OrderBy(bg => bg.Genre.Name)
                    .Select(bg => new GenreQueryResponse
                    {
                        Id = bg.Genre.Id,
                        Name = bg.Genre.Name
                    }).ToList()
            });
            return Task.FromResult(query);
        }
    }
}
