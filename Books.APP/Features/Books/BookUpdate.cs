using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookUpdateRequest : Record, IRequest<CommandResponse>
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

    public class BookUpdateHandler : DbService<Book>, IRequestHandler<BookUpdateRequest, CommandResponse>
    {
        public BookUpdateHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbQuery()
        {
            return base.DbQuery().Include(b => b.BookGenres);
        }

        public async Task<CommandResponse> Handle(BookUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(b => b.Id != request.Id && b.Name == request.Name.Trim() &&
                b.AuthorId == request.AuthorId, cancellationToken))
                return Error("Book with the same name exists for the author!");
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Book not found!");
            DbRemove(entity.BookGenres);
            entity.AuthorId = request.AuthorId.Value;
            entity.GenreIds = request.GenreIds;
            entity.IsTopSeller = request.IsTopSeller;
            entity.Name = request.Name.Trim();
            entity.NumberOfPages = request.NumberOfPages;
            entity.Price = request.Price.Value;
            entity.PublishDate = request.PublishDate.Value;
            await DbUpdateAsync(entity, cancellationToken);
            return Success(entity.Id, "Book updated successfully.");
        }
    }
}
