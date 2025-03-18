namespace CassieReplacement
{
    using Exiled.API.Features;
    using MEC;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Common functions used throughout the project.
    /// </summary>
    public static class CommonFuncs
    {
        private static AudioPlayer CassiePlayer => Plugin.CassiePlayer;

        private static Config Config => Plugin.Singleton.Config;

        /// <summary>
        /// Reads a message.
        /// </summary>
        /// <param name="messages">A list of strings to read.</param>
        public static void ReadMessage(List<string> messages)
        {
            float bg = 0f;
            foreach (string msg in messages)
            {
                if (Plugin.RegisteredClips.Where(c => c.Name == msg).Any())
                {
                    bg += Plugin.RegisteredClips.Where(c => c.Name == msg).FirstOrDefault().Length;
                }
            }

            int bgi = (int)Math.Round(bg);
            /*if (bgi < 0)
            {
                bgi = 0;
            }

            if (bgi > 36)
            {
                bgi = 36;
            }*/

            /*Plugin.CassiePlayer.AddClip($"bg_{bgi+4}", Config.CassieVolume);*/
            string a = string.Empty;
            for (int i = 0; i < bgi; i++)
            {
                a += " . .";
            }

            if (!Cassie.IsSpeaking)
            {
                Cassie.Clear();
                Timing.CallDelayed(3f, () =>
                {
                    Cassie.Message(a, false, true, true);
                    Timing.CallDelayed(2.25f, () => ReadWords(messages));
                });
            }
            else
            {
                Timing.CallDelayed(0.5f, () => ReadMessage(messages));
            }
        }

        private static void ReadWords(List<string> messages)
        {
            string msg = messages[0].ToLower();
            messages.Remove(msg);
            if (msg[0] == '.')
            {
                Timing.CallDelayed(0.25f, () => ReadWords(messages));
                return;
            }

            if (!AudioClipStorage.AudioClips.ContainsKey(msg))
            {
                ReadWords(messages);
            }

            CassiePlayer.AddClip(msg, Config.CassieVolume);
            Timing.CallDelayed(Plugin.GetClipLength(msg), () => ReadWords(messages));
        }
    }
}
