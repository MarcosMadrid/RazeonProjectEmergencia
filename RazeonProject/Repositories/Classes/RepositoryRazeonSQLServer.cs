using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazeonProject.Data;
using RazeonProject.Models;
using RazeonProject.Models.Relations;
using RazeonProject.Repositories.Interfaces;
using System.Data;
using System.Diagnostics.Metrics;

namespace RazeonProject.Repositories.Classes
{
    public class RepositoryRazeonSQLServer : IRepositoryRazeonBBDD
    {
        ContextRazeonBBDD razeonBBDD;
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader? reader;
        DataTable dataTable = new DataTable();

        public RepositoryRazeonSQLServer(ContextRazeonBBDD context
            , IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("RazeonSQLServer")!;
            this.connection = new SqlConnection(connectionString);
            razeonBBDD = context;
            command = connection.CreateCommand();
        }

        #region INSERT ALBUM PROCEDURE (SP_INSERT_ALBUM) 
        //ALTER PROCEDURE[dbo].[SP_INSERT_ALBUM]
        //(@ARTISTA INT, @NOMBRE NVARCHAR(50), @IMAGEN VARBINARY(MAX) )
        //AS
        //DECLARE

        //@ID_ALBUM INT;

        //SELECT @ID_ALBUM = COUNT(*) + 1  FROM Album;
        //INSERT INTO Album VALUES(@ID_ALBUM , @ARTISTA, @NOMBRE, NULL , @IMAGEN);
        #endregion
        public async Task<Album?> CreateAlbum(int idUser, string name, byte[] image)
        {
            string sql = "SP_INSERT_ALBUM @ARTISTA, @NOMBRE, @IMAGEN";
            
            SqlParameter sqlArtista = new SqlParameter("ARTISTA", idUser);
            SqlParameter sqlNombre = new SqlParameter("NOMBRE", name);
            SqlParameter sqlImagen = new SqlParameter("IMAGEN", image);

            await razeonBBDD.Database.ExecuteSqlRawAsync(sql, sqlArtista, sqlNombre, sqlImagen);
            string selectSql = "SELECT TOP(1) Album_ID, Artista_ID, Nombre, Imagen, Release_Date FROM Album ORDER BY Album_ID DESC";

            var result = await razeonBBDD.Albums.FromSqlRaw(selectSql).FirstOrDefaultAsync();
            return result;
        }

        #region INSERT TRACK PROCEDURE (SP_INSERT_CANCION)
        //CREATE OR ALTER PROCEDURE SP_INSERT_CANCION
        //(@ALBUM INT, @CANCION_IMG VARBINARY(MAX), @CANCION_FILE VARBINARY(MAX), @TITULO NVARCHAR(50))
        //AS
        //    DECLARE
        //      @IDCANCION INT;
        //    SELECT @IDCANCION = COUNT(*) + 1 FROM Cancion;
        //    INSERT INTO Cancion VALUES(@IDCANCION, @ALBUM, @TITULO, NULL, @CANCION_IMG, @CANCION_FILE, NULL)
        //GO
        #endregion
        public async Task<Track?> CreateTrack(int idAlbum, string title, byte[]? imgTrack, byte[]? fileTrack)
        {
            string insertSql = "SP_INSERT_CANCION @ALBUM, @CANCION_IMG, @CANCION_FILE, @TITULO";

            SqlParameter sqlAlbum = new SqlParameter("ALBUM", idAlbum);
            SqlParameter sqlNombre = new SqlParameter("TITULO", title);
            SqlParameter sqlImagen = new SqlParameter("CANCION_IMG", imgTrack);
            SqlParameter sqlFile = new SqlParameter("CANCION_FILE", fileTrack);

            await razeonBBDD.Database.ExecuteSqlRawAsync(insertSql, sqlAlbum, sqlImagen, sqlFile, sqlNombre);

            string selectSql = "SELECT TOP(1) Cancion_ID, Album_ID, Titulo, Duracion, RutaCancion, Cancion_Imagen, FileCancion FROM Cancion ORDER BY Cancion_ID DESC";

            var result = await razeonBBDD.Tracks.FromSqlRaw(selectSql).FirstOrDefaultAsync();
            return result;
        }

        public async Task DeleteTrack(int idTrack)
        {
            var trackToDelete = await razeonBBDD.Tracks.FindAsync(idTrack);
            razeonBBDD.Tracks.Remove(trackToDelete);
            await razeonBBDD.SaveChangesAsync();
        }

        #region VIEW V_ALLITEMS_OWNER 
   //     CREATE or ALTER VIEW V_ALLITEMS_OWNER
   //     AS
   //         SELECT Type, Item_ID, Titulo, Usuario.Nickname AS Owner, Owner_ID, Imagen
   //         FROM((SELECT'Cancion' AS Type, Cancion_ID AS Item_ID, Titulo AS Titulo, Album.Artista_ID AS Owner_ID , Cancion_Imagen as Imagen
   //                 FROM Cancion INNER JOIN Album ON Cancion.Album_ID = Album.Album_ID)
   //         UNION
   //         SELECT 'Playlist' AS Type, Playlist_ID AS Item_ID, Nombre AS Titulo, User_ID AS Owner_ID, Imagen_Playlist as Imagen
   //         FROM Playlist
   //         UNION
   //         SELECT 'Album' AS Type, Album_ID AS Item_ID, Nombre AS Titulo, Artista_ID AS Owner_ID, Imagen as Imagen
   //         FROM  Album
   //         )AS QUERY INNER JOIN Usuario ON User_ID = Owner_ID;
        #endregion
        public Items_Artist GetItemsUser(string User_ID)
        {
            Items_Artist items_artist = new Items_Artist();
            string sql = "SELECT * FROM V_ALLITEMS_OWNER WHERE Owner_ID = @User_ID";
            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.AddWithValue("User_ID", User_ID);

            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                string item_type = reader.GetString("Type");
                if (item_type.Equals("Album"))
                {
                    Album album = new Album
                    {
                        Id = reader.GetInt32("Item_ID"),
                        Name = reader.GetString("Titulo"),
                        ArtistId = reader.GetInt32("Owner_ID"),
                        Image = (byte[])reader["Imagen"]
                    };
                    items_artist.Albums.Add(album);
                }
                if (item_type.Equals("Playlist"))
                {
                    Playlist playlist = new Playlist
                    {
                        Id = reader.GetInt32("Item_ID"),
                        Name = reader.GetString("Titulo"),
                        UserId = reader.GetInt32("Owner_ID"),
                        Image = (byte[])reader["Imagen"]
                    };
                    items_artist.Playlists.Add(playlist);
                }
                if (item_type.Equals("Cancion"))
                {
                    Track track = new Track
                    {
                        Id = reader.GetInt32("Item_ID"),
                        Title = reader.GetString("Titulo"),
                        Image = (byte[])reader["Imagen"]
                    };
                    items_artist.Tracks.Add(track);
                }
            }
            connection.Close();
            command.Parameters.Clear();

            return items_artist;
        }

        public async Task<List<Track>?> GeAlbumTracks(int idAlbum)
        {
            return
                await razeonBBDD.Tracks.Where(track=> track.AlbumId == idAlbum).ToListAsync();
        }

        public User? GetUser(string User_ID)
        {
            User? user = null;
            string sql = "SELECT * FROM USUARIO WHERE @User_ID = User_ID;";
            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.AddWithValue("User_ID", User_ID);

            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("User_ID")),
                    Name = reader.GetString(reader.GetOrdinal("Nickname")),
                    BirthDate = null,
                    Image = reader.IsDBNull(reader.GetOrdinal("Perfil_Imagen")) ? null : (byte[])reader["Perfil_Imagen"],
                    RolId = reader.GetInt32(reader.GetOrdinal("Rol_ID"))
                };
            }
            connection.Close();
            command.Parameters.Clear();

            return user;
        }

        public User? GetUserByEmail(string email)
        {
            User? user = null;
            string sql = "SELECT * FROM USUARIO WHERE @Email = Email;";
            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.AddWithValue("Email", email);

            connection.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("User_ID")),
                    Name = reader.GetString(reader.GetOrdinal("Nickname")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    BirthDate = null,
                    Image = reader.IsDBNull(reader.GetOrdinal("Perfil_Imagen")) ? null : (byte[])reader["Perfil_Imagen"],
                    RolId = reader.GetInt32(reader.GetOrdinal("Rol_ID"))
                };
            }
            connection.Close();
            command.Parameters.Clear();

            return user;
        }

        public async Task<User?> LogIn(string email, string password)
        {
            var user = razeonBBDD.Users.FirstOrDefaultAsync(userRow => userRow.Email == email && userRow.Password == password);
            return await user;
        }

        #region INSERT USER PROCEDURE (SIGN UP)
        //USE[RAZEON]
        //GO
        ///****** Object:  StoredProcedure [dbo].[SP_INSERT_USER]    Script Date: 02/03/2024 19:36:24 ******/
        //SET ANSI_NULLS ON
        //GO
        //SET QUOTED_IDENTIFIER ON
        //GO
        //ALTER   PROCEDURE [dbo].[SP_INSERT_USER]
        //(@NICKNAME NVARCHAR(50), @EMAIL NVARCHAR(50), @PASSWORD NVARCHAR(50), @IMAGE VARBINARY(max), @USER_ROL NVARCHAR(50))
        //AS
        //BEGIN
        //	DECLARE 
        //		@USERID INT,
        //        @USER_ROL_ID INT;

        //SELECT @USER_ROL_ID = Rol_ID FROM Rol_User WHERE @USER_ROL = Rol_User.Tipo;
        //IF @USER_ROL_ID IS NULL
        //    BEGIN
        //        RAISERROR('User role does not exist.', 16, 1);
        //RETURN;
        //END;

        //SELECT @USERID = (MAX(User_ID) + 1) FROM Usuario;
        //IF @USERID IS NULL
        //    BEGIN
        //        SET @USERID = 1;
        //END;

        //INSERT INTO Usuario VALUES ( @USERID , @NICKNAME, @EMAIL, @PASSWORD, NULL, @IMAGE, @USER_ROL_ID);
        //IF @@ROWCOUNT = 0
        //    BEGIN
        //        RAISERROR('Failed to insert userRow.', 16, 1);
        //RETURN;
        //END;
        //END
        #endregion
        public bool SignUp(string nickname, string email, string password, byte[] image, int user_rol)
        {
            string sql = "SP_INSERT_USER";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("NICKNAME", nickname);
            command.Parameters.AddWithValue("EMAIL", email);
            command.Parameters.AddWithValue("PASSWORD", password);
            command.Parameters.AddWithValue("IMAGE", image);
            command.Parameters.AddWithValue("USER_ROL_ID", user_rol);

            command.CommandText = sql;
            command.CommandType = CommandType.StoredProcedure;

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            command.Parameters.Clear();

            return true;
        }

        public async Task<Album?> UpdateAlbum(int idAlbum, int idUser, string name, byte[] image)
        {
            var albumToUpdate = await razeonBBDD.Albums.FindAsync(idAlbum);

            if (albumToUpdate != null)
            {
                albumToUpdate.Name = name;
                albumToUpdate.Image = image;

                await razeonBBDD.SaveChangesAsync();

                return albumToUpdate;
            }
            else
            {
                return null;
            }
        }

        public async  Task<Track?> GeTrack(int idTrack)
        {
            return
                await razeonBBDD.Tracks.FirstOrDefaultAsync(track => track.Id == idTrack);
        }

        #region PROCEDURE SP_PAGINATION_ARTISTS
        //CREATE OR ALTER PROCEDURE SP_PAGINATION_ARTISTS
        //(@PAGE_NUMBER INT, @RECORDS_PER_PAGE INT, @TOTAL_RECORDS INT OUTPUT)
        //AS
        //BEGIN
        //    SELECT @TOTAL_RECORDS = COUNT(*) FROM Usuario WHERE Rol_ID = 2;
        //        DECLARE
        //            @OFFSET INT = (@PAGE_NUMBER - 1) * @RECORDS_PER_PAGE;
        //    SELECT ISNULL(User_ID, 0),
        //            Nickname,
        //            Email,
        //            Password,
        //            Date_of_Birth,
        //            Perfil_Imagen,
        //            Rol_ID
        //    FROM(
        //        SELECT
        //            ROW_NUMBER() OVER (ORDER BY User_ID) AS RowNum, User_ID, Nickname, Email, Password, Date_of_Birth, Perfil_Imagen, Rol_ID

        //                FROM
        //                    Usuario

        //    ) AS PaginationQuery
        //    WHERE
        //        RowNum BETWEEN @OFFSET + 1 AND @OFFSET + @RECORDS_PER_PAGE and Rol_ID = 2;
        //END;
        #endregion
        public async Task<(List<User>, int)> GetArtistsPagination(int pageNumber, int recordsPerPage)
        {
            string sql = "EXECUTE SP_PAGINATION_ARTIST @PAGE_NUMBER, @RECORDS_PER_PAGE, @TOTAL_RECORDS OUTPUT";
            var totalRecordsParameter = new SqlParameter
            {
                ParameterName = "@TOTAL_RECORDS",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            SqlParameter sqlPage = new SqlParameter("@PAGE_NUMBER", pageNumber);
            SqlParameter sqlRecords = new SqlParameter("@RECORDS_PER_PAGE", recordsPerPage);

            var artists = await razeonBBDD.Users.FromSqlRaw(sql, sqlPage, sqlRecords ,totalRecordsParameter).ToListAsync();

            int totalRecords = (int)totalRecordsParameter.Value;

            return (artists, totalRecords);
        }
    }
}
