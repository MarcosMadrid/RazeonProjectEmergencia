using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazeonProject.Models
{
    [Table("Playlist")]
    public class Playlist
    {
        [Key]
        [Required]
        [Column("Playlist_ID")]
        [DataType("interger")]
        public required int Id { get; set; }

        [Required]
        [Column("User_ID")]
        [DataType("interger")]
        public required int UserId { get; set; }

        [Required]
        [Column("Nombre")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }

        [Column("Imagen_Playlist")]
        [DataType(DataType.Upload)]
        public byte[]? Image { get; set; }

    }
}
