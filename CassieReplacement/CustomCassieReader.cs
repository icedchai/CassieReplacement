namespace CassieReplacement
{
    using CassieReplacement.Models;
    using MEC;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Reads Custom CASSIE messages.
    /// </summary>
    public class CustomCassieReader
    {
        private static int ticksSinceCassieSpoke = 0;

        private string currentPrefix = string.Empty;

        private string currentSuffix = string.Empty;

        /// <summary>
        /// Gets the <see cref="CustomCassieReader"/> singleton.
        /// </summary>
        public static CustomCassieReader Singleton { get; internal set; } = new CustomCassieReader();

        private AudioPlayer CassiePlayer => Plugin.CassiePlayer;

        private AudioPlayer CassiePlayerGlobal => Plugin.CassiePlayerGlobal;

        private Config Config => Plugin.Singleton.Config;

        /// <summary>
        /// Checks every frame whether CASSIE is speaking.
        /// </summary>
        /// <returns>IEnumerator float.</returns>
        public static IEnumerator<float> CassieCheck()
        {
            while (true)
            {
                if (NineTailedFoxAnnouncer.singleton.queue.Count != 0)
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
        /// <param name="isNoisy">A value indicating whether to put this message to CASSIE background noise.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        public void CassieReadMessage(List<string> messages, bool isNoisy = true, string translation = "")
        {
            ReadMessage(messages, new List<AudioPlayer> { CassiePlayer, CassiePlayerGlobal }, isNoisy, translation);
        }

        /// <summary>
        /// Reads a message from CustomCassie, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The words to read.</param>
        /// <param name="isNoisy">A value indicating whether to put this message to CASSIE background noise.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        public void CassieReadMessage(string messages, bool isNoisy = true, string translation = "")
        {
            ReadMessage(messages.Split(' ').ToList(), new List<AudioPlayer> { CassiePlayer, CassiePlayerGlobal }, isNoisy, translation);
        }

        /// <summary>
        /// Reads a message from an <see cref="AudioPlayer"/> instance, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The list of strings to read.</param>
        /// <param name="audioPlayers">The <see cref="AudioPlayer"/> instances to play this message from.</param>
        /// <param name="isNoisy">Value indicating whether to add a CASSIE background to this message reading.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        public void ReadMessage(List<string> messages, List<AudioPlayer> audioPlayers, bool isNoisy = false, string translation = "")
        {
            float bg = 0f;
            string baseCassieAnnouncement = string.Empty;
            List<CassieClip> clipsToRegister = new List<CassieClip>();
            for (int i = 0; i < messages.Count(); i++)
            {
                string msg = messages[i];
                if (string.IsNullOrWhiteSpace(msg))
                {
                    messages.Remove(msg);
                    i--;
                    continue;
                }

                if (msg.StartsWith("prefix_", true, null) || msg.StartsWith("suffix_", true, null))
                {
                    switch (msg.Remove(6).ToLower())
                    {
                        case "prefix":
                            currentPrefix = msg.Remove(0, 7);
                            break;
                        case "suffix":
                            currentSuffix = msg.Remove(0, 7);
                            break;
                        default:
                            break;
                    }

                    messages.Remove(msg);
                    i--;
                    continue;
                }

                msg = $"{currentPrefix}{msg}{currentSuffix}";
                messages[i] = msg;
                if (Plugin.RegisteredClips.Where(c => c.Name == msg).Any())
                {
                    CassieClip currentClip = Plugin.RegisteredClips.Where(c => c.Name == msg).First();

                    clipsToRegister.Add(currentClip);
                    AudioClipStorage.LoadClip(currentClip.FileInfo.FullName, currentClip.Name);

                    int howManyDotsToAdd = (int)Math.Round(currentClip.Length * 2, MidpointRounding.AwayFromZero);
                    for (int j = 0; j < howManyDotsToAdd; j++)
                    {
                        baseCassieAnnouncement += " .";
                    }

                    bg += currentClip.Length;
                }
                else
                {
                    baseCassieAnnouncement += $" {msg}";
                }
            }

            currentPrefix = string.Empty;
            currentSuffix = string.Empty;

            if (!isNoisy)
            {
                RespawnEffectsController.PlayCassieAnnouncement(string.IsNullOrWhiteSpace(translation) ? baseCassieAnnouncement : $"{translation.Replace(' ', '\u2005')}<size=0> {baseCassieAnnouncement} </size>", false, isNoisy, !string.IsNullOrWhiteSpace(translation));
                ReadWords(messages, audioPlayers);
                return;
            }

            currentPrefix = string.Empty;
            currentSuffix = string.Empty;


            if (ticksSinceCassieSpoke <= 360)
            {
                Timing.CallDelayed(0.5f, () => ReadMessage(messages, audioPlayers: audioPlayers, translation: translation, isNoisy: isNoisy));
            }
            else
            {
                RespawnEffectsController.PlayCassieAnnouncement(string.IsNullOrWhiteSpace(translation) ? baseCassieAnnouncement : $"{translation.Replace(' ', '\u2005')}<size=0> {baseCassieAnnouncement} </size>", false, true, !string.IsNullOrWhiteSpace(translation));
                Timing.CallDelayed(2.25f, () => ReadWords(messages, audioPlayers, clipsToRegister));
            }
        }

        private void ReadWords(List<string> messages, List<AudioPlayer> audioPlayers, List<CassieClip> clipsToUnregister = null)
        {
            if (messages.Count == 0)
            {
                if (clipsToUnregister is not null)
                {
                    foreach (var clip in clipsToUnregister)
                    {
                        AudioClipStorage.DestroyClip(clip.Name);
                    }
                }

                return;
            }


            string msg = messages[0].ToLower();
            messages.Remove(msg);

            if (!AudioClipStorage.AudioClips.ContainsKey(msg) || !Plugin.RegisteredClips.Where(c => c.Name == msg).Any())
            {
                Timing.CallDelayed(NineTailedFoxAnnouncer.singleton.CalculateDuration(msg), () => ReadWords(messages, audioPlayers, clipsToUnregister));
                return;
            }

            foreach (AudioPlayer audioPlayer in audioPlayers)
            {
                audioPlayer.AddClip(msg, Config.CassieVolume);
            }

            Timing.CallDelayed(Plugin.GetClipLength(msg), () => ReadWords(messages, audioPlayers, clipsToUnregister));
        }
    }
}
