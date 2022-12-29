using Chinook.Models;
using Playlist = Chinook.ClientModels.Playlist;
using Track = Chinook.ClientModels.PlaylistTrack;

namespace Chinook.Services.Contracts
{
    public interface IPlaylistService
    {
        Task<List<Artist>> GetArtists();
        Task<List<Album>> GetAlbumsForArtist(int artistId);
        Playlist GetPlayList(long playListId, string userId);
        Artist GetArtistbyId(long artistId);
        List<Track> GetTracks(long artistId, string userId);
        void SavePlayList(Playlist playlist, string userId);
        List<Playlist> GetUserCreatedPlayList(string userId);
        void RemoveTrack(long trackId);
        void EditPlayList(long playlistId, string name);
        void DeletePlayList(long playlistId);
        void FavouriteTrack(long trackId);
        void UnFavouriteTrack(long trackId);
    }
}
