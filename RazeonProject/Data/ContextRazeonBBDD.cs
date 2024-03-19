using Microsoft.EntityFrameworkCore;
using RazeonProject.Models;

namespace RazeonProject.Data
{
    public class ContextRazeonBBDD : DbContext
    {
        public ContextRazeonBBDD(DbContextOptions options) : base(options) { }

        public DbSet<Album>? Albums { get; set; }
        public DbSet<Track>? Tracks { get; set; }
        public DbSet<Playlist>? Playlists { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
