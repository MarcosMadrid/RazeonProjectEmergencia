using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazeonProject.Models
{
    [Table("Usuario")]
    public class User
    {
        [Key]
        [Required]
        [Column("User_ID")]
        [DataType("interger")]
        public int Id { get; set; }

        [Required]
        [Column("Nickname")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Required]
        [Column("Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [Column("Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Column("Date_of_Birth")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required]
        [Column("Perfil_Imagen")]
        public byte[]? Image { get; set; }

        [Required]
        [Column("Rol_ID")]
        public int RolId { get; set; }
    }
}
