using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazeonProject.Models
{
    [Table("Cancion")]
    public class Track
    {
        [Key]
        [Required]
        [Column("Cancion_ID")]
        [DataType("interger")]
        public int Id { get; set; }

        [Column("Album_ID")]
        [DataType("interger")]
        public int AlbumId { get; set; }
    
        [Column("Titulo")]
        [DataType(DataType.Text)]
        public string? Title { get; set; }

        //[Column("Duracion")]
        //[DataType(DataType.Time)]
        //public TimeSpan? Duration { get; set; }

        //[Column("RutaCancion")]
        //[DataType(DataType.Text)]
        //public string? RutaCancion  { get; set; }

        [Column("Cancion_Imagen")]
        public byte[]? Image { get; set; }

        [Column("FileCancion")]        
        public byte[]? FileAudio { get; set; }
    }
}
