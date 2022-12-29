using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using Playlist = Chinook.ClientModels.Playlist;
using Track = Chinook.ClientModels.PlaylistTrack;

namespace Chinook.Services
{
    public class PlaylistService: IPlaylistService
    {
        private readonly ChinookContext chinookContext;
        public PlaylistService(ChinookContext chinookContext) {
            this.chinookContext = chinookContext;
        }

        public async Task<List<Artist>> GetArtists()
        {
            //var users = await chinookContext.Users.Include(a => a.UserPlaylists).ToListAsync();
          
            return await  chinookContext.Artists.ToListAsync();
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
           
            return await chinookContext.Albums.Where(a => a.ArtistId == artistId).ToListAsync();
        }

        public Playlist GetPlayList(long playListId, string userId)
        {
            return chinookContext.Playlists
            .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
            .Where(p => p.PlaylistId == playListId)
            .Select(p => new ClientModels.Playlist()
            {
                Name = p.Name,
                Tracks = p.Tracks.Select(t => new ClientModels.PlaylistTrack()
                {
                    AlbumTitle = t.Album.Title,
                    ArtistName = t.Album.Artist.Name,
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsDeleted = t.IsDeleted,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "Favorites")).Any()
                }).Where(s => !s.IsDeleted).ToList()
            })
            .FirstOrDefault();
        }
        public List<Playlist> GetUserCreatedPlayList(string userId)
        {
            return chinookContext.UserPlaylists
            .Include(a => a.Playlist)
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .Select(p => new ClientModels.Playlist()
            {
                PlaylistId= p.PlaylistId,
                Name = p.Playlist.Name
            })
            .ToList();

        }
        public List<Track> GetTracks(long artistId, string userId)
        {
            return chinookContext.Tracks.Where(a => a.Album.ArtistId == artistId && !a.IsDeleted)
            .Include(a => a.Album)
            .Select(t => new PlaylistTrack()
            {
                AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                TrackId = t.TrackId,
                TrackName = t.Name,
                IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == userId && up.Playlist.Name == "Favorites")).Any()
            })
            .ToList();
        }

        public Artist GetArtistbyId(long artistId)
        {
            return chinookContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);
        }

        public void SavePlayList(Playlist playlist, string userId)
        {
            Random random = new Random();
            Chinook.Models.Playlist ply = new Chinook.Models.Playlist();
            ply.Name = playlist.Name;
            ply.PlaylistId = random.Next();
            chinookContext.Playlists.Add(ply);
            chinookContext.SaveChanges();

            chinookContext.Entry(ply).Reload();
            foreach (var track in playlist.Tracks)
            {
                var trackadd = chinookContext.Tracks.Where(s => s.TrackId == track.TrackId).SingleOrDefault();
                ply.Tracks.Add(trackadd);
            }
            Chinook.Models.UserPlaylist userPlaylist = new Chinook.Models.UserPlaylist();
            userPlaylist.UserId = userId;
            userPlaylist.PlaylistId = ply.PlaylistId;
            chinookContext.UserPlaylists.Add(userPlaylist);

            
            chinookContext.SaveChanges();
        }

        public void RemoveTrack(long trackId)
        {
            var  track=chinookContext.Tracks.Where(s => s.TrackId == trackId).Single();
            track.IsDeleted = true;
            chinookContext.Entry(track).State = EntityState.Modified;
            chinookContext.SaveChanges();
        }

        public void EditPlayList(long playlistId, string name)
        {
            var playList = chinookContext.Playlists.Where(s => s.PlaylistId == playlistId).Single();
            playList.Name = name;
            chinookContext.Playlists.Update(playList);
            chinookContext.SaveChanges();
        }

        public void DeletePlayList(long playlistId)
        {
            var playList = chinookContext.Playlists.Where(s => s.PlaylistId == playlistId).Single();
            playList.IsDeleted = true;
            chinookContext.Playlists.Update(playList);
            chinookContext.SaveChanges();
            
        }

        public void FavouriteTrack(long trackId)
        {
            var playList = chinookContext.Playlists.Include(a => a.Tracks).Where(s =>  s.Name == "Favorites").Single();
            var track = chinookContext.Tracks.Where(s => s.TrackId == trackId).Single();
            playList.Tracks.Add(track);
        }
        public void UnFavouriteTrack(long trackId)
        {
            var playList = chinookContext.Playlists.Include(a => a.Tracks).Where(s => s.Name == "Favorites").Single();
            var track = chinookContext.Tracks.Where(s => s.TrackId == trackId).Single();
            playList.Tracks.Remove(track);
        }
    }
}
