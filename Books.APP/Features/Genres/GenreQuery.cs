using CORE.Domain;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Books.APP.Domain;

namespace Books.APP.Features.Genres
{
    public class GenreQueryRequest : Record, IRequest<IQueryable<GenreQueryResponse>>
    {
    }

    public class GenreQueryResponse : Record
    {
        public string Name { get; set; }

        public int BookCountR { get; set; }

        public string BookNamesR { get; set; }
    }

    public class GenreQueryHandler : DbService<Genre>, IRequestHandler<GenreQueryRequest, IQueryable<GenreQueryResponse>>
    {
        public GenreQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Genre> DbQuery()
        {
            return base.DbQuery().Include(g => g.BookGenres).ThenInclude(bg => bg.Book).OrderBy(g => g.Name);
        }

        public Task<IQueryable<GenreQueryResponse>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(DbQuery().Select(g => new GenreQueryResponse
            {
                Id = g.Id,
                Name = g.Name,

                BookCountR = g.BookGenres.Count,
                BookNamesR = string.Join(", ", g.BookGenres.Select(bg => bg.Book.Name))
            }));
        }
    }
}
