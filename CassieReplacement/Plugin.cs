namespace CassieReplacement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CassieReplacement.Models;
    using CassieReplacement.Patches;
    using HarmonyLib;
    using MapGeneration;
    using MEC;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp079;
#if EXILED
    using Exiled.API.Features;
#else
    using LabApi.Loader.Features.Plugins;
    using LabApi.Features;
    using LabApi.Features.Wrappers;
#endif
    /// <inheritdoc/>
    public class Plugin : Plugin<Config>
    {
        public static AudioPlayer CassiePlayer { get; private set; }

        public static AudioPlayer CassiePlayerGlobal { get; private set; }

        public static AudioPlayer RadioPlayer { get; private set; }

        /// <summary>
        /// The singleton.
        /// </summary>
        public static Plugin Singleton;

        /// <inheritdoc/>
        public override string Name => "CASSIE Replacement";

#if EXILED
        /// <inheritdoc/>
        public override string Prefix => "cassie_replacement";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(9, 6, 0);

        private CassieEventHandlers cassieEventHandlers { get; set; }
#else
        /// <inheritdoc/>
        public override string Description => "CASSIE replacement plugin";

        /// <inheritdoc/>
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
#endif

        /// <inheritdoc/>
        public override string Author => "icedchqi";

        /// <inheritdoc/>
        public override Version Version => new(1, 5, 0);

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

                if (Config.UseSpatialSpeakers || !p.TryGetCurrentRoom(out RoomIdentifier room))
                {
                    return true;
                }

                IEnumerable<Scp079InteractableBase> speakers = Scp079Speaker.AllInstances.Where(s => s is Scp079Speaker && Room.Get(s.Room) == Room.Get(room));
                bool ret = speakers.IsEmpty() || speakers.Any(s => UnityEngine.Vector3.Distance(p.GetPosition(), s.Position) > Config.SpatialSpeakerMaxDistance);
                return ret;
            };
            CassiePlayerGlobal.AddSpeaker("Main", isSpatial: false, maxDistance: 50000f, volume: Config.GlobalSpeakerVolume);
            CassiePlayer = AudioPlayer.CreateOrGet("icedchqi_cassieplayer");
            int i = 0;
            foreach (Scp079InteractableBase speaker in Scp079InteractableBase.AllInstances.Where(i => i is Scp079Speaker))
            {
                i++;
                CassiePlayer.AddSpeaker($"speaker_{i}", speaker.Position, volume: Config.SpatialSpeakerVolume, minDistance: Config.SpatialSpeakerMinDistance, maxDistance: Config.SpatialSpeakerMaxDistance);
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
#if EXILED

        public override void OnEnabled()
        {
            base.OnEnabled();
            cassieEventHandlers = new();
            cassieEventHandlers.Register();
#else
        public override void Enable()
        {
#endif
            Singleton = this;
            CustomCassieReader.Singleton = new CustomCassieReader();
            Patcher.DoPatching();
            foreach (CassieDirectory configDir in Config.BaseDirectories)
            {
                RegisterFolder(configDir);
            }

            Timing.CallDelayed(10f, () =>
            {
                Timing.RunCoroutine(CustomCassieReader.CassieCheck());
            });

            LabApi.Events.Handlers.ServerEvents.RoundStarted += InitSpeaker;
        }

        /// <inheritdoc/>
#if EXILED

        public override void OnDisabled()
        {
            base.OnDisabled();
            cassieEventHandlers.Unregister();
            cassieEventHandlers = null;
#else
        public override void Disable()
        {
#endif
            Singleton = null;
            CustomCassieReader.Singleton = null;
            Harmony harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.UnpatchAll("me.icedchai.cassie.patch");
            UnregisterClips();

            LabApi.Events.Handlers.ServerEvents.RoundStarted -= InitSpeaker;
        }

        /// <summary>
        /// Gets a clip with the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The first <see cref="CassieClip"/> registered whose name is the same as <paramref name="name"/>, or null.</returns>
        public static CassieClip GetClip(string name)
        {
            name = name.ToLower();
            IEnumerable<CassieClip> clips = registeredClips.Where(c => c.Name == name);
            return clips.FirstOrDefault();
        }

        /// <summary>
        /// Gets the length of the specified clip.
        /// </summary>
        /// <param name="clipName">The name of the clip in question.</param>
        /// <returns>A float representing how long a plugin-registered clip is, minus <see cref="Config.CassieReverb"/>, or zero.</returns>
        public static float GetClipLength(string clipName)
        {
            if (GetClip(clipName) is not null)
            {
                return GetClip(clipName).Length;
            }

            return 0f;
        }

        /// <summary>
        /// Gets the base length of the specified clip.
        /// </summary>
        /// <param name="clipName">The name of the clip in question.</param>
        /// <returns>A float representing how long a plugin-registered clip is, or zero.</returns>
        public static float GetClipBaseLength(string clipName)
        {
            if (GetClip(clipName) is not null)
            {
                return GetClip(clipName).BaseLength;
            }

            return 0f;
        }

        internal static void RegisterFolder(CassieDirectory directoryConfiguration, string directory = null)
        {
#if EXILED
            directoryConfiguration.Path = directoryConfiguration.Path.Replace("{exiled_config}", Paths.Configs);
#endif
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
                while (RegisteredClipNames.Contains(cassieClip.Name))
                {
                    cassieClip.Name += "_";
                }

                registeredClips.Add(cassieClip);
            }
        }

        internal static void UnregisterClips()
        {
            registeredClips.Clear();
        }
    }
}
