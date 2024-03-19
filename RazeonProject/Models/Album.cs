using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazeonProject.Models
{
    [Table("Album")]
    public class Album
    {
        [Key]
        [Required]
        [Column("Album_ID")]
        [DataType("interger")]
        public int Id { get; set; }

        [Column("Artista_ID")]
        [DataType("interger")]
        public int ArtistId { get; set; }

        [Column("Nombre")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Column("Release_Date")]
        public DateTime? Released { get; set; }

        [Column("Imagen")]
        [DataType(DataType.Upload)]
        public byte[]? Image { get; set; }
    }
}
