using CORE.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.APP.Domain
{
    public class Book : Record
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        public short? NumberOfPages { get; set; }

        public DateTime PublishDate { get; set; }

        public decimal Price { get; set; }

        public bool IsTopSeller { get; set; }

        public int AuthorId { get; set; } // foreign key

        public Author Author { get; set; } // navigation property

        public List<BookGenre> BookGenres { get; set; } = new(); // navigation property

        [NotMapped]
        public List<int> GenreIds
        {
            get => BookGenres.Select(bg => bg.GenreId).ToList();
            set => BookGenres = value.Select(gId => new BookGenre
            {
                GenreId = gId
            }).ToList();
        }
    }
}
