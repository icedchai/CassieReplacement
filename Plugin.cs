namespace CassieReplacement
{
    using CassieReplacement.Models;
    using CassieReplacement.Patches;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
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
        public override Version Version => new (1, 3, 0);

        private static List<CassieClip> registeredClips = new List<CassieClip>();

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/>'s.
        /// </summary>
        public static List<CassieClip> RegisteredClips => registeredClips;

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/> names.
        /// </summary>
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
            Patcher.DoPatching();
            foreach (CassieDirectory configDir in Config.BaseDirectories)
            {
                RegisterFolder(configDir);
            }

            Timing.CallDelayed(10f, () =>
            {
                Timing.RunCoroutine(CommonFuncs.CassieCheck());
            });

            Exiled.Events.Handlers.Server.RoundStarted += CommonFuncs.InitSpeaker;
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Singleton = null;
            Harmony harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.UnpatchAll("me.icedchai.cassie.patch");
            foreach (CassieClip clip in registeredClips)
            {
                AudioClipStorage.DestroyClip(clip.Name);
                registeredClips.Remove(clip);
            }

            Exiled.Events.Handlers.Server.RoundStarted -= CommonFuncs.InitSpeaker;
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

        private void RegisterFolder(CassieDirectory directoryConfiguration, string directory = null)
        {
            DirectoryInfo d = new DirectoryInfo(directoryConfiguration.Path);
            if (directory is not null)
            {
                d = new DirectoryInfo(directory);
            }

            if (!d.Exists)
            {
                return;
            }

            foreach (DirectoryInfo directoryInfo in d.GetDirectories())
            {
                RegisterFolder(directoryConfiguration, directoryInfo.FullName);
            }

            foreach (FileInfo file in d.GetFiles("*.ogg"))
            {
                CassieClip cassieClip = new CassieClip(file, directoryConfiguration.BleedTime, directoryConfiguration.Prefix);

                // Prevent duplicates from being registered by appending _ to the name as needed.
                while (AudioClipStorage.AudioClips.ContainsKey(cassieClip.Name))
                {
                    cassieClip.Name += "_";
                }

                registeredClips.Add(cassieClip);
                AudioClipStorage.LoadClip(cassieClip.FileInfo.FullName, cassieClip.Name);

                Log.Debug($"Registered {cassieClip.Name}, at {cassieClip.FileInfo.FullName}, length {cassieClip.Length}");
            }
        }
    }
}
