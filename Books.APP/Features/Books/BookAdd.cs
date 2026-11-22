using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookAddRequest : Record, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(150, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string Name { get; set; }

        [Range(1, short.MaxValue, ErrorMessage = "{0} must be between {1} and {2}!")]
        public short? NumberOfPages { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public DateTime? PublishDate { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Range(0, double.MaxValue, ErrorMessage = "{0} must be a positive value!")]
        public decimal? Price { get; set; }

        public bool IsTopSeller { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public int? AuthorId { get; set; }

        public List<int> GenreIds { get; set; } = new();
    }

    public class BookAddHandler : DbService<Book>, IRequestHandler<BookAddRequest, CommandResponse>
    {
        public BookAddHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(BookAddRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(b => b.Name == request.Name.Trim() &&
                b.AuthorId == request.AuthorId, cancellationToken))
                return Error("Book with the same name exists for the author!");
            var entity = new Book
            {
                AuthorId = request.AuthorId.Value,
                GenreIds = request.GenreIds,
                IsTopSeller = request.IsTopSeller,
                Name = request.Name.Trim(),
                NumberOfPages = request.NumberOfPages,
                Price = request.Price.Value,
                PublishDate = request.PublishDate.Value
            };
            await DbAddAsync(entity, cancellationToken);
            return Success(entity.Id, "Book added successfully.");
        }
    }
}
