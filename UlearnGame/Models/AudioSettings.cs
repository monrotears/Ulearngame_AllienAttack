using Microsoft.Xna.Framework.Media;

namespace UlearnGame.Models
{
    public static class AudioSettings
    {
        public static float MenuVolume { get; set; } = 0.1f;

        public static float GameVolume { get; set; } = 0.4f;

        public static bool IsMuted { get; set; }

        public static float CurrentMenuVolume
        {
            get { return IsMuted ? 0f : MenuVolume; }
        }

        public static float CurrentGameVolume
        {
            get { return IsMuted ? 0f : GameVolume; }
        }

        public static void ApplyMenuVolume()
        {
            MediaPlayer.Volume = CurrentMenuVolume;
        }
    }
}
