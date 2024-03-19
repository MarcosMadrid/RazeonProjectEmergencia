using Microsoft.AspNetCore.Mvc;
using RazeonProject.Models;
using RazeonProject.Repositories.Interfaces;
using System.Text.Json;

namespace RazeonProject.Controllers
{
    public class ProviderController : Controller
    {
        IRepositoryRazeonBBDD razeonBBDD;

        public ProviderController(IRepositoryRazeonBBDD razeonBBDD)
        {
            this.razeonBBDD = razeonBBDD;
        }

        public IActionResult GetPlaylistView()
        {
            return View();
        }

        public async Task<IActionResult> LoadSong(int idTrack)
        {
            Track? track = await razeonBBDD.GeTrack(idTrack);

            if (track != null)
            {
                return File(track.FileAudio!, "audio/mp3");
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> LoadSongImg(int idTrack)
        {
            Track? track = await razeonBBDD.GeTrack(idTrack);

            if (track != null)
            {
                return File(track.Image!, "image/png");
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> GetSongLoaded(int idTrack)
        {
            Track? track = await razeonBBDD.GeTrack(idTrack);

            if (track != null)
            {
                return Ok(track);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> LoadAlbumSongs(int idAlbum)
        {
            List<Track>? tracks = await razeonBBDD.GeAlbumTracks(idAlbum);

            if (tracks != null && tracks.Any())
            {
                try
                {
                    List<string> base64Tracks = new List<string>();

                    foreach (Track track in tracks)
                    {
                        byte[] audioBytes = track.FileAudio;
                        if (audioBytes != null && audioBytes.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(audioBytes);
                            base64Tracks.Add(base64String);
                        }
                    }

                    return Ok(base64Tracks);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult LoadImg(int id, string type)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "imgs" ,type);
            return Ok(filePath);
        }
    }
}
