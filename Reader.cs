namespace CassieReplacement
{
    using Exiled.API.Features;
    using MEC;
    using PlayerRoles;
    using Respawning;
    using Subtitles;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Utils.Networking;

    /// <summary>
    /// Common functions used throughout the project.
    /// </summary>
    public static class Reader
    {
        private static AudioPlayer CassiePlayer => Plugin.CassiePlayer;

        private static Config Config => Plugin.Singleton.Config;

        private static int ticksSinceCassieSpoke = 0;

        /// <summary>
        /// Checks every frame whether CASSIE is speaking.
        /// </summary>
        /// <returns>IEnumerator float.</returns>
        public static IEnumerator<float> CassieCheck()
        {
            while (true)
            {
                if (Cassie.IsSpeaking)
                {
                    ticksSinceCassieSpoke = 0;
                }
                else
                {
                    ticksSinceCassieSpoke++;
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        /// <summary>
        /// Reads a message from CustomCassie, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The list of strings to read.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        public static void CassieReadMessage(List<string> messages, string translation = "")
        {
            ReadMessage(messages, CassiePlayer, true, translation);
        }

        /// <summary>
        /// Reads a message from an <see cref="AudioPlayer"/> instance, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The list of strings to read.</param>
        /// <param name="audioPlayer">The <see cref="AudioPlayer"/> instance to play this message from.</param>
        /// <param name="isNoisy">Value indicating whether to add a CASSIE background to this message reading.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        public static void ReadMessage(List<string> messages, AudioPlayer audioPlayer, bool isNoisy = false, string translation = "")
        {
            float bg = 0f;
            foreach (string msg in messages)
            {
                if (Plugin.RegisteredClips.Where(c => c.Name == msg).Any())
                {
                    bg += Plugin.RegisteredClips.Where(c => c.Name == msg).FirstOrDefault().Length;
                }
            }

            if (!isNoisy)
            {
                ReadWords(messages, audioPlayer);
                return;
            }

            // Calculates the empty CASSIE message to send.
            int bgi = (int)Math.Round(bg);
            string a = string.Empty;
            for (int i = 0; i < bgi; i++)
            {
                a += " . .";
            }

            if (ticksSinceCassieSpoke <= 360)
            {
                Timing.CallDelayed(0.5f, () => ReadMessage(messages, audioPlayer: audioPlayer, translation: translation, isNoisy: isNoisy));
            }
            else
            {
                RespawnEffectsController.PlayCassieAnnouncement(string.IsNullOrWhiteSpace(translation) ? a : $"{translation.Replace(' ', '\u2005')}<size=0>{a}</size>", false, true, !string.IsNullOrWhiteSpace(translation));
                Timing.CallDelayed(2.25f, () => ReadWords(messages, audioPlayer));
            }
        }

        private static void ReadWords(List<string> messages, AudioPlayer audioPlayer)
        {
            string msg = messages[0].ToLower();
            messages.Remove(msg);
            if (msg[0] == '.')
            {
                Timing.CallDelayed(0.25f, () => ReadWords(messages, audioPlayer));
                return;
            }

            if (!AudioClipStorage.AudioClips.ContainsKey(msg))
            {
                ReadWords(messages, audioPlayer);
                return;
            }

            audioPlayer.AddClip(msg, Config.CassieVolume);
            Timing.CallDelayed(Plugin.GetClipLength(msg), () => ReadWords(messages, audioPlayer));
        }
    }
}
