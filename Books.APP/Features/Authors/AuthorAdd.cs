using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorAddRequest : Record, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string LastName { get; set; }
    }

    public class AuthorAddHandler : DbService<Author>, IRequestHandler<AuthorAddRequest, CommandResponse>
    {
        public AuthorAddHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(AuthorAddRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(a => a.FirstName == request.FirstName.Trim() &&
                a.LastName == request.LastName.Trim(), cancellationToken))
                return Error("Author with the same first name and last name exists!");
            var entity = new Author
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim()
            };
            await DbAddAsync(entity, cancellationToken);
            return Success(entity.Id, "Author added successfully.");
        }
    }
}
