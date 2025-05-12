namespace CassieReplacement
{
    using CassieReplacement.Models;
    using CassieReplacement.Patches;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using MapGeneration;
    using MEC;
    using NVorbis;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp079;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using UnityEngine.Rendering;

    /// <inheritdoc/>
    public class Plugin : Exiled.API.Features.Plugin<Config>
    {
        public static AudioPlayer CassiePlayer { get; private set; }

        public static AudioPlayer CassiePlayerGlobal { get; private set; }

        public static AudioPlayer RadioPlayer { get; private set; }

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
        public override Version Version => new (1, 4, 0);

        private static List<CassieClip> registeredClips = new List<CassieClip>();

        /// <summary>
        /// Destroys the speaker on <see cref="Plugin.CassiePlayer"/>, and then re-adds it.
        /// </summary>
        public void InitSpeaker()
        {
            CassiePlayerGlobal = AudioPlayer.CreateOrGet("icedchqi_cassieplayer_global");
            CassiePlayerGlobal.Condition = p =>
            {
                if (p == null || p.IsHost)
                {
                    return false;
                }

                if (!p.TryGetCurrentRoom(out RoomIdentifier room))
                {
                    return true;
                }

                IEnumerable<Scp079InteractableBase> speakers = Scp079Speaker.AllInstances.Where(s => Room.Get(s.Room) == Room.Get(room));
                bool ret = speakers.IsEmpty() || speakers.Any(s => UnityEngine.Vector3.Distance(p.GetPosition(), s.Position) > 4f);
                return ret;
            };
            CassiePlayerGlobal.AddSpeaker("Main", isSpatial: false, maxDistance: 50000f, volume: 1.5f);
            CassiePlayer = AudioPlayer.CreateOrGet("icedchqi_cassieplayer");
            int i = 0;
            foreach (Scp079InteractableBase speaker in Scp079Speaker.AllInstances)
            {
                i++;
                IEnumerable<Speaker> closeSpeakers = CassiePlayer.SpeakersByName.Values.Where(s => UnityEngine.Vector3.Distance(s.Position, speaker.Position) < 5);
                if (closeSpeakers.Any())
                {
                    continue;
                }

                CassiePlayer.AddSpeaker($"speaker_{i}", speaker.Position, minDistance: 1, maxDistance: 10);
            }
        }

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/>'s.
        /// </summary>
        public static List<CassieClip> RegisteredClips => registeredClips;

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/> names.
        /// </summary>
        public static List<string> RegisteredClipNames => registeredClips.Select(c => c.Name).ToList();

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
                Timing.RunCoroutine(CustomCassieReader.CassieCheck());
            });

            Exiled.Events.Handlers.Server.RoundStarted += InitSpeaker;
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

            Exiled.Events.Handlers.Server.RoundStarted -= InitSpeaker;
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
