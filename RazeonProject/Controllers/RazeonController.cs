using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RazeonProject.Repositories.Interfaces;
using RazeonProject.Filters;
using RazeonProject.Models.Relations;
using RazeonProject.Models;
using System.Text.Json;

namespace RazeonProject.Controllers
{
    [AuthorizeUserAttribute]
    [ServiceFilter(typeof(GlobalBuilderView))]
    //[ServiceFilter(typeof(SendRazeonLayout))]
    public class RazeonController : Controller
    {
        IRepositoryRazeonBBDD razeonBBDD;
        IMemoryCache memoryCache;

        public RazeonController(IRepositoryRazeonBBDD repository, IMemoryCache memoryCache)
        {
            razeonBBDD = repository;
            this.memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return PartialView("_PlayerLayout");
        }

        public IActionResult Profile(string User_ID)
        {
            User? user = razeonBBDD.GetUser(User_ID);

            Items_Artist items_Artist = razeonBBDD.GetItemsUser(User_ID);
            ViewData["items"] = JsonSerializer.Serialize(items_Artist);
            return PartialView("Profile/_Profile", user);
        }

        [HttpGet]
        public IActionResult FormAlbum()
        {
            return PartialView("FormAlbum/_FormAlbum");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetStatePlayer([FromBody] Player player)
        {
            if (player != null)
            {
                memoryCache.Set("PLAYER", player);
            }
            return Ok(player);
        }

        public IActionResult PlayerPartialView()
        {
            Player? player;
            memoryCache.TryGetValue("PLAYER", out player);
            if (player is null)
            {
                player = new Player();
            }

            return PartialView("Player/_Player");
        }

        public IActionResult Home()
        {
            return PartialView("Home/_Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlbum(Album album, IFormFile Image)
        {
            try
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        Image.CopyTo(memoryStream);
                        album.Image = memoryStream.ToArray();
                    }
                }
                Album? albumResult = await razeonBBDD.CreateAlbum(album.ArtistId, album.Name!, album.Image!);
                if (albumResult != null)
                {
                    return Ok(albumResult);
                }
                return BadRequest(albumResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack(Track track, IFormFile Image, IFormFile FileAudio)
        {
            try
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        Image.CopyTo(memoryStream);
                        track.Image = memoryStream.ToArray();
                    }
                }
                if (FileAudio != null && FileAudio.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        FileAudio.CopyTo(memoryStream);
                        track.FileAudio = memoryStream.ToArray();
                    }
                }
                Track? trackResult = await razeonBBDD.CreateTrack(track.AlbumId, track.Title!, track.Image!, track.FileAudio);
                if (trackResult != null)
                {
                    return Ok(trackResult);
                }
                return BadRequest(trackResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTrack(int idTrack)
        {
            try
            {
                await razeonBBDD.DeleteTrack(idTrack);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbumTracks(int idAlbum)
        {
            List<Track>? tracks = await razeonBBDD.GeAlbumTracks(idAlbum);
            return PartialView("_TableItems", tracks);
        }

        [HttpPut]
        public async Task<IActionResult> EditAlbum(Album album, IFormFile Image)
        {

            if (Image != null && Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    Image.CopyTo(memoryStream);
                    album.Image = memoryStream.ToArray();
                }
            }
            Album? albumResult = await razeonBBDD.UpdateAlbum(album.Id, album.ArtistId, album.Name!, album.Image!);
            if (albumResult != null)
            {
                return Ok(albumResult);
            }
            else
            {
                return BadRequest(albumResult);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetArtistPage(int? page, int records)
        {
            records = 4;
            if (page == null)
            {
                page = 1;
            }
            var artists = await razeonBBDD.GetArtistsPagination(page!.Value, records);
            return PartialView("_ArtistPartialView", artists);
        }
    }
}
