namespace CassieReplacement.Reader
{
    using CassieReplacement;
    using CassieReplacement.Config;
    using CassieReplacement.Reader.Models;
    using MEC;
    using NVorbis;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using UnityEngine;
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

        private Dictionary<string, CassieClip> PitchShiftedTempClips { get; set; } = new Dictionary<string, CassieClip>();

        public ClipDatabase ClipDatabase { get; set; } = new ();

        private CassieClip GetClip(string name)
        {
            name = name.ToLower();
            CassieClip clip = ClipDatabase.GetClip(name);
            if (clip is not null)
            {
                return clip;
            }
            else
            {
                PitchShiftedTempClips.TryGetValue(name, out clip);
                return clip;
            }

        }

        private float GetClipLength(string clipName)
        {
            CassieClip clip = GetClip(clipName);
            if (clip is not null)
            {
                return clip.Length;
            }

            return 0f;
        }

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

        /// <summary>
        /// Gets or sets a list of <see cref="AudioPlayer"/>'s from which all announcements fed into this reader will be played.
        /// </summary>
        public List<AudioPlayer> AudioPlayers { get; set; }

        /// <summary>
        /// Gets or sets <see cref="DateTime"/> that all announcements which started before it should be terminated.
        /// </summary>
        internal DateTime TimeBeforeWhichToPause { get; set; } = DateTime.MinValue;

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
            Timing.RunCoroutine(ReadMessage(messages, AudioPlayers, isNoisy, customAnnouncement, translation, useCassie));
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
            Timing.RunCoroutine(ReadMessage(messages.Split(' ').ToList(), AudioPlayers, isNoisy, customAnnouncement, translation, useCassie));
        }

        // I did not write this.
        private static float[] Resample(float[] inputBuffer, int inputSampleRate, int outputSampleRate)
        {
            double sampleRateRatio = (double)outputSampleRate / inputSampleRate;
            int outputBufferLength = (int)(inputBuffer.Length * sampleRateRatio);

            float[] outputBuffer = new float[outputBufferLength];

            for (int i = 0; i < outputBufferLength; i++)
            {
                double position = i / sampleRateRatio;
                int leftIndex = (int)Math.Floor(position);
                int rightIndex = leftIndex + 1;

                double fraction = position - leftIndex;

                if (rightIndex >= inputBuffer.Length)
                {
                    outputBuffer[i] = inputBuffer[leftIndex];
                }
                else
                {
                    outputBuffer[i] = (float)(inputBuffer[leftIndex] * (1 - fraction) + inputBuffer[rightIndex] * fraction);
                }
            }

            return outputBuffer;
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
        public IEnumerator<float> ReadMessage(List<string> messages, List<AudioPlayer> audioPlayers, bool isNoisy = false, bool customAnnouncement = true, string translation = "", bool useCassie = true)
        {
            string baseCassieAnnouncement = string.Empty;
            HashSet<CassieClip> clipsToUnregister = new HashSet<CassieClip>();

            float pitch = 1.0f;

            Task firstWordTask = null;

            // Process the messages & register sounds before proceeding.
            for (int i = 0; i < messages.Count(); i++)
            {
                string msg = messages[i].ToLower();
                if (string.IsNullOrWhiteSpace(msg))
                {
                    messages.Remove(msg);
                    i--;
                    continue;
                }

                if (msg.StartsWith("pitch_"))
                {
                    pitch = float.TryParse(msg.Remove(0, "pitch_".Length), out float newPitch) ? newPitch : pitch;
                    continue;
                }

                // prefix/suffix processor
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

                // Gets the registered clip
                CassieClip msgCassieClip = ClipDatabase.RegisteredClips.FirstOrDefault(c => c.Name == msg);

                if (msgCassieClip is not null)
                {
                    // Creates a new one if pitch shifted.
                    if (pitch != 1.0f)
                    {
                        string newName = $"{pitch}_{msgCassieClip.Name}";
                        if (!PitchShiftedTempClips.TryGetValue(newName, out CassieClip pitched))
                        {
                            msgCassieClip = new CassieClip(newName, msgCassieClip.FileInfo, msgCassieClip.BaseLength / pitch, msgCassieClip.Reverb / pitch);
                            PitchShiftedTempClips.Add(newName, msgCassieClip);
                            msg = msgCassieClip.Name;
                        }
                        else
                        {
                            msgCassieClip = pitched;
                            msg = pitched.Name;
                        }
                    }

                    messages[i] = msg;

                    // Part of dynamic registration; when a clip is added here, it will be unregistered when the reader is done reading the message.
                    clipsToUnregister.Add(msgCassieClip);

                    if (pitch == 1.0f)
                    {
                        if (!AudioClipStorage.AudioClips.ContainsKey(msgCassieClip.Name))
                        {
                            Task task = Task.Run(() => AudioClipStorage.LoadClip(msgCassieClip.FileInfo.FullName, msgCassieClip.Name));

                            if (firstWordTask is null)
                            {
                                firstWordTask = task;
                            }
                        }
                    }
                    else
                    {
                        // Prevents modifications to pitch in subsequent iterations from interfering with the following task.
                        float workingPitch = pitch;
                        Task task = Task.Run(() =>
                        {
                            int sampleRate;
                            int num;
                            float[] array;
                            using (VorbisReader vorbisReader = new VorbisReader(msgCassieClip.FileInfo.FullName))
                            {
                                sampleRate = vorbisReader.SampleRate;
                                num = vorbisReader.Channels;
                                array = new float[vorbisReader.TotalSamples * num];
                                vorbisReader.ReadSamples(array);
                            }

                            array = Resample(array, 48000, Convert.ToInt32(48000 / workingPitch));

                            AudioClipStorage.AudioClips.Add(msgCassieClip.Name, new AudioClipData(msgCassieClip.Name, sampleRate, num, array));
                        });

                        if (firstWordTask is null)
                        {
                            firstWordTask = task;
                        }
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
                while (firstWordTask is not null && !firstWordTask.IsCompleted)
                {
                    yield return Timing.WaitForOneFrame;
                }

                HandlesToMessages.Add(Timing.RunCoroutine(ReadWords(messages, audioPlayers, clipsToUnregister)), messages);
                yield break;
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
                while (firstWordTask is not null && !firstWordTask.IsCompleted)
                {
                    yield return Timing.WaitForOneFrame;
                }

                RespawnEffectsController.PlayCassieAnnouncement(CassieAnnouncement.MessageTranslated(baseCassieAnnouncement, string.IsNullOrWhiteSpace(translation) ? string.Join(" ", messages) : translation), false, isNoisy, customAnnouncement);
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

            DateTime timeStarted = DateTime.Now;
            int jamDelay = 0;
            int jamAmount = 0;
            float pitch = 1.0f;

            foreach (string msg in messages)
            {
                if (TimeBeforeWhichToPause >= timeStarted)
                {
                    break;
                }

                if (msg.StartsWith("pitch_"))
                {
                    pitch = float.TryParse(msg.Remove(0, "pitch_".Length), out float newPitch) ? newPitch : pitch;
                    continue;
                }

                if (msg.StartsWith("yield_"))
                {
                    if (float.TryParse(msg.Remove(0, "yield_".Length), out float yield))
                    {
                        yield return Timing.WaitForSeconds(yield);
                    }

                    continue;
                }

                if (!AudioClipStorage.AudioClips.ContainsKey(msg) || (!ClipDatabase.RegisteredClips.Any(c => c.Name == msg) && !PitchShiftedTempClips.TryGetValue(msg, out _)))
                {
                    yield return Timing.WaitForSeconds(NineTailedFoxAnnouncer.singleton.CalculateDuration(msg) / pitch);
                    continue;
                }

                foreach (AudioPlayer audioPlayer in audioPlayers)
                {
                    float volume = Config.CassieVolume;

                    if (audioPlayer == Plugin.CassiePlayerGlobal)
                    {
                        volume *= Config.GlobalSpeakerVolumeMultiplier;
                    }

                    AudioClipPlayback playback = audioPlayer.AddClip(msg, volume);
                }

                yield return Timing.WaitForSeconds(GetClipLength(msg));
            }

            yield return Timing.WaitForSeconds(GetClip(messages.Last()).Reverb);
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
