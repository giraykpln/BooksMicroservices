using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Books.APP.Domain;

namespace Books.APP.Features.Authors
{
    public class AuthorUpdateRequest : Record, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(50, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string LastName { get; set; }
    }

    public class AuthorUpdateHandler : DbService<Author>, IRequestHandler<AuthorUpdateRequest, CommandResponse>
    {
        public AuthorUpdateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(AuthorUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(a => a.Id != request.Id &&
                a.FirstName == request.FirstName.Trim() &&
                a.LastName == request.LastName.Trim(), cancellationToken))
                return Error("Author with the same first name and last name exists!");
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Author not found!");
            entity.FirstName = request.FirstName.Trim();
            entity.LastName = request.LastName.Trim();
            await DbUpdateAsync(entity, cancellationToken);
            return Success(entity.Id, "Author updated successfully.");
        }
    }
}
