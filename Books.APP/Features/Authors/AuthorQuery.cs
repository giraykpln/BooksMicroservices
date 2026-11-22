using CORE.Domain;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorQueryRequest : Record, IRequest<IQueryable<AuthorQueryResponse>>
    {
    }

    public class AuthorQueryResponse : Record
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullNameR { get; set; }

        public int BookCountR { get; set; }

        public string BookNamesR { get; set; }
    }

    public class AuthorQueryHandler : DbService<Author>, IRequestHandler<AuthorQueryRequest, IQueryable<AuthorQueryResponse>>
    {
        public AuthorQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Author> DbQuery()
        {
            return base.DbQuery().Include(a => a.Books).OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
        }

        public Task<IQueryable<AuthorQueryResponse>> Handle(AuthorQueryRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(DbQuery().Select(a => new AuthorQueryResponse
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,

                FullNameR = a.FirstName + " " + a.LastName,
                BookCountR = a.Books.Count,
                BookNamesR = string.Join(", ", a.Books.Select(b => b.Name))
            }));
        }
    }
}
