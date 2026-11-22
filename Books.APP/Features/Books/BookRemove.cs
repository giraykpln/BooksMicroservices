using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Books
{
    public class BookRemoveRequest : Record, IRequest<CommandResponse>
    {
    }

    public class BookRemoveHandler : DbService<Book>, IRequestHandler<BookRemoveRequest, CommandResponse>
    {
        public BookRemoveHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbQuery() => base.DbQuery().Include(b => b.BookGenres);

        public async Task<CommandResponse> Handle(BookRemoveRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Book not found!");
            DbRemove(entity.BookGenres);
            await DbRemoveAsync(entity, cancellationToken);
            return Success(entity.Id, "Book deleted successfully.");
        }
    }
}
