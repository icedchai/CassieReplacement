﻿namespace CassieReplacement
{
    using Exiled.API.Features;
    using MEC;
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
    public static class CommonFuncs
    {
        private static AudioPlayer CassiePlayer
        {
            get
            {
                return Plugin.CassiePlayer;
            }

            set
            {
                Plugin.CassiePlayer = value;
            }
        }

        private static Config Config => Plugin.Singleton.Config;

        private static int ticksSinceCassieSpoke = 0;

        /// <summary>
        /// Destroys the speaker on <see cref="Plugin.CassiePlayer"/>, and then re-adds it.
        /// </summary>
        public static void InitSpeaker()
        {
            if (AudioPlayer.TryGet($"icedchqi_cassieplayer", out AudioPlayer player))
            {
                if (player is not null)
                {
                    player.Destroy();
                }
            }

            CassiePlayer = AudioPlayer.Create("icedchqi_cassieplayer");
            CassiePlayer.AddSpeaker("Main", isSpatial: false, maxDistance: 5000f);

        }

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
        /// Reads a message.
        /// </summary>
        /// <param name="messages">A list of strings to read.</param>
        public static void ReadMessage(List<string> messages, AudioPlayer audioPlayer = null, string translation = "")
        {
            if (audioPlayer is null)
            {
                audioPlayer = CassiePlayer;
            }

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

            if (ticksSinceCassieSpoke <= 360)
            {
                Timing.CallDelayed(0.5f, () => ReadMessage(messages, audioPlayer: audioPlayer, translation: translation));
            }
            else
            {
                RespawnEffectsController.PlayCassieAnnouncement(string.IsNullOrWhiteSpace(translation) ? a : $"{translation.Replace(' ', '\u2005')}<size=0>{a}</size>", false, true, !string.IsNullOrWhiteSpace(translation));
                Timing.CallDelayed(2.25f, () => ReadWords(messages, audioPlayer));
            }
        }

        private static void ReadWords(List<string> messages, AudioPlayer audioPlayer = null)
        {
            if (audioPlayer is null)
            {
                audioPlayer = CassiePlayer;
            }

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
