namespace CassieReplacement
{
    using CassieReplacement.Models;
    using Exiled.API.Features;
    using MEC;
    using NVorbis;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <inheritdoc/>
    public class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public static AudioPlayer CassiePlayer;

        /// <summary>
        /// The singleton.
        /// </summary>
        public static Plugin Singleton;

        /// <summary>
        /// Gets the singleton's config.
        /// </summary>
        public static Config PluginConfig => Singleton.Config;

        /// <inheritdoc/>
        public override string Name => "CASSIE Replacement";

        /// <inheritdoc/>
        public override string Prefix => "cassie_replacement";

        /// <inheritdoc/>
        public override string Author => "icedchqi";

        /// <inheritdoc/>
        public override Version Version => new (1, 0, 0);

        private static List<CassieClip> registeredClips = new List<CassieClip>();

        public static List<CassieClip> RegisteredClips => registeredClips;

        public static List<string> RegisteredClipNames
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (CassieClip clip in registeredClips)
                {
                    ret.Add(clip.Name);
                }

                return ret;
            }
        }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();
            Singleton = this;

            foreach (string configDir in Config.BaseDirectories)
            {
                // Ensure that a null or non-existent directory does not get through.
                string directory = string.IsNullOrWhiteSpace(configDir) || !Directory.Exists(configDir) ? Paths.Configs : configDir;
                RegisterFolder(directory);
            }

            Timing.CallDelayed(10f, () =>
            {
                CassiePlayer = AudioPlayer.CreateOrGet($"icedchqi_cassieplayer", onIntialCreation: (p) =>
                {
                    // This created speaker will be in 2D space ( audio will be always playing directly on you not from specific location ) but make sure that max distance is set to some higher value.
                    Speaker speaker = p.AddSpeaker("Main", isSpatial: false, maxDistance: 5000f);
                });
            });
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
        }

        /// <summary>
        /// Gets the length of the specified clip.
        /// </summary>
        /// <param name="clipName">The name of the clip in question.</param>
        /// <returns>A float representing how long a plugin-registered clip is, minus <see cref="Config.CassieReverb"/>, or zero.</returns>
        public static float GetClipLength(string clipName)
        {
            clipName = clipName.ToLower();
            IEnumerable<CassieClip> clips = registeredClips.Where(c => c.Name == clipName);
            if (clips.Any())
            {
                return clips.FirstOrDefault().Length;
            }

            return 0f;
        }

        private void RegisterFolder(string directory)
        {
            DirectoryInfo d = new DirectoryInfo(directory);

            foreach (FileInfo file in d.GetFiles("*.ogg"))
            {
                CassieClip cassieClip = new CassieClip(file);
                registeredClips.Add(cassieClip);

                // Prevent duplicates from being registered by appending _ to the name as needed.
                while (AudioClipStorage.AudioClips.ContainsKey(cassieClip.Name))
                {
                    cassieClip.Name += "_";
                }

                AudioClipStorage.LoadClip(cassieClip.FileInfo.FullName, cassieClip.Name);

                Log.Debug($"Registered {cassieClip.Name}, at {cassieClip.FileInfo.FullName}, length {cassieClip.Length}");
            }
        }
    }
}
