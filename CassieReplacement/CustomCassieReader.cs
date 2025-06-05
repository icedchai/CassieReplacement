namespace CassieReplacement
{
    using CassieReplacement.Models;
    using MEC;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils.NonAllocLINQ;

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
        /// Gets a lookup table of coroutine handles to the messages they're in progress of reading.
        /// </summary>
        internal Dictionary<CoroutineHandle, List<string>> HandlesToMessages { get; set; } = new Dictionary<CoroutineHandle, List<string>>();

        private bool IsBeingUsed(string name)
        {
            foreach (var kvp in HandlesToMessages)
            {
                if (kvp.Key.IsRunning && kvp.Value.Contains(name))
                {
                    return true;
                }

                if (!kvp.Key.IsRunning)
                {
                    HandlesToMessages.Remove(kvp.Key);
                }
            }

            return false;
        }

        internal bool ShouldPause { get; set; } = false;

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
        /// <param name="customAnnouncement">Value indicating whether to add a subtitle to this message reading.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        /// <param name="useCassie">Value indicating whether to use CASSIE basegame message.</param>
        public void CassieReadMessage(List<string> messages, bool isNoisy = true, bool customAnnouncement = true, string translation = "", bool useCassie = true)
        {
            ReadMessage(messages, Plugin.Singleton.CassieAudioPlayers, isNoisy, customAnnouncement, translation, useCassie);
        }

        /// <summary>
        /// Reads a message from CustomCassie, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The words to read.</param>
        /// <param name="isNoisy">A value indicating whether to put this message to CASSIE background noise.</param>
        /// <param name="customAnnouncement">Value indicating whether to add a subtitle to this message reading.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        /// <param name="useCassie">Value indicating whether to use CASSIE basegame message.</param>
        public void CassieReadMessage(string messages, bool isNoisy = true, bool customAnnouncement = true, string translation = "", bool useCassie = true)
        {
            ReadMessage(messages.Split(' ').ToList(), Plugin.Singleton.CassieAudioPlayers, isNoisy, customAnnouncement, translation, useCassie);
        }

        /// <summary>
        /// Reads a message from an <see cref="AudioPlayer"/> instance, using the registered audio clips.
        /// </summary>
        /// <param name="messages">The list of strings to read.</param>
        /// <param name="audioPlayers">The <see cref="AudioPlayer"/> instances to play this message from.</param>
        /// <param name="isNoisy">Value indicating whether to add a CASSIE background to this message reading.</param>
        /// <param name="customAnnouncement">Value indicating whether to add a subtitle to this message reading.</param>
        /// <param name="translation">The CASSIE subtitles to use.</param>
        /// <param name="useCassie">Value indicating whether to use CASSIE basegame message.</param>
        public void ReadMessage(List<string> messages, List<AudioPlayer> audioPlayers, bool isNoisy = false, bool customAnnouncement = true, string translation = "", bool useCassie = true)
        {
            string baseCassieAnnouncement = string.Empty;
            HashSet<CassieClip> clipsToUnregister = new HashSet<CassieClip>();
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
                CassieClip msgCassieClip = Plugin.RegisteredClips.FirstOrDefault(c => c.Name == msg);
                if (msgCassieClip is not null)
                {
                    // Part of dynamic registration; when a clip is added here, it will be unregistered when the reader is done reading the message.
                    clipsToUnregister.Add(msgCassieClip);
                    if (!AudioClipStorage.AudioClips.ContainsKey(msgCassieClip.Name))
                    {
                        AudioClipStorage.LoadClip(msgCassieClip.FileInfo.FullName, msgCassieClip.Name);
                    }

                    if (useCassie)
                    {
                        // Adds the appropriate amount of dots, where each dot is ~0.5 seconds
                        int howManyDotsToAdd = (int)Math.Round(msgCassieClip.Length * 2, MidpointRounding.AwayFromZero);
                        for (int j = 0; j < howManyDotsToAdd; j++)
                        {
                            baseCassieAnnouncement += " .";
                        }
                    }
                }
                else if (useCassie)
                {
                    baseCassieAnnouncement += $" {msg}";
                }
            }

            currentPrefix = string.Empty;
            currentSuffix = string.Empty;

            if (!useCassie)
            {
                HandlesToMessages.Add(Timing.RunCoroutine(ReadWords(messages, audioPlayers, clipsToUnregister)), messages);
                return;
            }

            if (Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAll)
            {
                baseCassieAnnouncement = "noparse " + baseCassieAnnouncement;
            }

            if (ticksSinceCassieSpoke <= 360)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    ReadMessage(messages, audioPlayers: audioPlayers, translation: translation, isNoisy: isNoisy);
                });
            }
            else
            {
                RespawnEffectsController.PlayCassieAnnouncement(string.IsNullOrWhiteSpace(translation) ? $"{string.Join(" ", messages).Replace(' ', '\u2005')}<size=0> {baseCassieAnnouncement} </size>" : $"{translation.Replace(' ', '\u2005')}<size=0> {baseCassieAnnouncement} </size>", false, isNoisy, customAnnouncement);
                Timing.CallDelayed(isNoisy ? 2.25f : 0, () =>
                {
                    HandlesToMessages.Add(Timing.RunCoroutine(ReadWords(messages, audioPlayers, clipsToUnregister)), messages);
                });
            }
        }

        private IEnumerator<float> ReadWords(List<string> messages, List<AudioPlayer> audioPlayers, HashSet<CassieClip> clipsToUnregister = null)
        {
            if (messages.Count == 0)
            {
                yield break;
            }

            ShouldPause = false;
            foreach (string msg in messages)
            {
                if (ShouldPause)
                {
                    yield break;
                }

                if (!AudioClipStorage.AudioClips.ContainsKey(msg) || !Plugin.RegisteredClips.Any(c => c.Name == msg))
                {
                    yield return Timing.WaitForSeconds(NineTailedFoxAnnouncer.singleton.CalculateDuration(msg));
                }

                foreach (AudioPlayer audioPlayer in audioPlayers)
                {
                    if (!AudioClipStorage.AudioClips.ContainsKey(msg))
                    {

                    }

                    audioPlayer.AddClip(msg, Config.CassieVolume);
                }

                yield return Timing.WaitForSeconds(Plugin.GetClipLength(msg));
            }

            yield return Timing.WaitForSeconds(Plugin.GetClipBaseLength(messages.Last()));
            if (clipsToUnregister is not null)
            {
                foreach (var clip in clipsToUnregister)
                {
                    if (!IsBeingUsed(clip.Name))
                    {
                        AudioClipStorage.DestroyClip(clip.Name);
                    }
                }
            }
        }
    }
}
