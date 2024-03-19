using RazeonProject.Models;
using RazeonProject.Models.Relations;
using System.Threading.Tasks;

namespace RazeonProject.Repositories.Interfaces
{
    public interface IRepositoryRazeonBBDD
    {
        Task<User?> LogIn(string email, string password);
        Boolean SignUp(string nickname, string email, string password, byte[] image, int user_rol);
        Items_Artist GetItemsUser(string User_ID);
        Task<(List<User>, int)> GetArtistsPagination(int pageNumber, int recordsPerPage);
        Task<List<Track>?> GeAlbumTracks(int idAlbum);
        Task<Track?> GeTrack(int idTrack);
        Task DeleteTrack(int idTrack);
        User? GetUser(string User_ID);
        User? GetUserByEmail(string email);
        Task<Album?> CreateAlbum(int idUser, string name, byte[] image);
        Task<Album?> UpdateAlbum(int idAlbum, int idUser, string name, byte[] image);
        Task<Track?> CreateTrack(int idAlbum , string title, byte[]? imgTrack, byte[]? fileTrack);
    }
}
