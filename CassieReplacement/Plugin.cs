namespace CassieReplacement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CassieReplacement.Config;
    using CassieReplacement.Patches;
    using HarmonyLib;
    using MapGeneration;
    using MEC;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp079;
#if EXILED
    using Exiled.API.Features;
    using CassieReplacement.Reader;
    using CassieReplacement.Reader.Models;
    using System.Threading.Tasks;
#else
    using LabApi.Loader.Features.Plugins;
    using LabApi.Features;
    using LabApi.Features.Wrappers;
#endif

    /// <inheritdoc/>
    public class Plugin : Plugin<Config.Config>
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
        public override Version Version => new(1, 6, 0);

        private static List<CassieClip> registeredClips = new List<CassieClip>();

        private IEnumerable<Scp079InteractableBase> allSpeakers;

        /// <summary>
        /// Gets a list of all <see cref="AudioPlayer"/>'s registered in <see cref="InitSpeaker"/>.
        /// </summary>
        internal List<AudioPlayer> CassieAudioPlayers { get; private set; } = new List<AudioPlayer>();

        /// <summary>
        /// Destroys the speaker on <see cref="Plugin.CassiePlayer"/>, and then re-adds it.
        /// </summary>
        public void InitSpeaker()
        {
            CassieAudioPlayers.Clear();
            allSpeakers = Scp079Speaker.AllInstances.Where(s => s is Scp079Speaker);
            if (Config.UseGlobalSpeaker)
            {
                CassiePlayerGlobal = AudioPlayer.CreateOrGet("icedchqi_cassieplayer_global");
                CassiePlayerGlobal.Condition = p =>
                {
                    p.TryGetCurrentRoom(out RoomIdentifier room);
                    if (p == null || p.IsHost)
                    {
                        return false;
                    }

                    if (!Config.UseSpatialSpeakers || (Config.GlobalForSurfaceOnly && room.Zone == FacilityZone.Surface))
                    {
                        return true;
                    }

                    IEnumerable<Scp079InteractableBase> speakers = allSpeakers.Where(s => Room.Get(s.Room) == Room.Get(room));
                    bool ret = speakers.IsEmpty() || (speakers.Any(s => UnityEngine.Vector3.Distance(p.PlayerCameraReference.position, s.Position) >= Config.SpatialSpeakerMaxDistance) ^ room is null);
                    return ret;
                };
                CassiePlayerGlobal.AddSpeaker("Main", isSpatial: false, minDistance: 50000f, maxDistance: 50000f, volume: Config.GlobalSpeakerVolume);
                CassieAudioPlayers.Add(CassiePlayerGlobal);
            }

            if (Config.UseSpatialSpeakers)
            {
                CassiePlayer = AudioPlayer.CreateOrGet("icedchqi_cassieplayer");
                int i = 0;
                foreach (Scp079InteractableBase speaker in allSpeakers)
                {
                    if (Config.GlobalForSurfaceOnly && speaker.Room.Zone == FacilityZone.Surface)
                    {
                        continue;
                    }

                    i++;
                    CassiePlayer.AddSpeaker($"speaker_{i}", speaker.Position, volume: Config.SpatialSpeakerVolume, minDistance: Config.SpatialSpeakerMinDistance, maxDistance: Config.SpatialSpeakerMaxDistance);
                }

                CassieAudioPlayers.Add(CassiePlayer);
            }

            CustomCassieReader.Singleton.AudioPlayers = CassieAudioPlayers;
        }

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
            Task.Run(() =>
            {
                foreach (CassieDirectorySerializable configDir in Config.BaseDirectories)
                {
                    CustomCassieReader.Singleton.ClipDatabase.RegisterFolder(configDir);
                }
            });

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
            Harmony harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.UnpatchAll("me.icedchai.cassie.patch");

            LabApi.Events.Handlers.ServerEvents.RoundStarted -= InitSpeaker;
            Singleton = null;
            CustomCassieReader.Singleton = null;
        }
    }
}
