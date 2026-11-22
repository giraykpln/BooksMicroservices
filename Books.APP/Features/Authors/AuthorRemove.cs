using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorRemoveRequest : Record, IRequest<CommandResponse>
    {
    }

    public class AuthorRemoveHandler : DbService<Author>, IRequestHandler<AuthorRemoveRequest, CommandResponse>
    {
        public AuthorRemoveHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Author> DbQuery()
        {
            return base.DbQuery().Include(a => a.Books);
        }

        public async Task<CommandResponse> Handle(AuthorRemoveRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Author not found!");
            if (entity.Books.Any())
                return Error("Author can't be deleted because it has relational books!");
            await DbRemoveAsync(entity, cancellationToken);
            return Success(entity.Id, "Author deleted successfully.");
        }
    }
}
