using Chinook.ClientModels;

namespace Chinook.Services
{
    public class PlaylistNameChangeService
    {
        public List<Playlist> Value { get; set; }
        public Playlist singleValue { get; set; }

        public event Action OnStateChange;
        public void SetValue(List<Playlist> value)
        {
            this.Value = value;
            NotifyStateChanged();
        }

        public void SetSingleValue(Playlist singleValue)
        {
            this.singleValue = singleValue;
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnStateChange?.Invoke();
    }
}
