namespace RazeonProject.Models.Relations
{
    public class Items_Artist
    {
        string ArtistName = string.Empty;
        public List<Playlist> Playlists { get; set; }
        public List<Track> Tracks { get; set; }
        public List<Album> Albums { get; set; }

        public Items_Artist()
        {
            Playlists = new List<Playlist>();
            Albums = new List<Album>();
            Tracks = new List<Track>();
        }
    }
}
