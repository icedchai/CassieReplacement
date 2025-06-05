namespace CassieReplacement.Config
{
#pragma warning disable SA1600
    using System.Collections.Generic;
    using System.ComponentModel;
#if EXILED
    using Exiled.API.Interfaces;
    using CassieReplacement.Reader.Models;

    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

#else

    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class Config
    {
#endif
        public bool UseGlobalSpeaker { get; set; } = true;

        public bool UseSpatialSpeakers { get; set; } = false;

        public float SpatialSpeakerMaxDistance { get; set; } = 10f;

        public float SpatialSpeakerMinDistance { get; set; } = 8f;

        public float SpatialSpeakerVolume { get; set; } = 1f;

        public float SurfaceZoneSpatialMultiplier { get; set; } = 3f;

        public float GlobalSpeakerVolume { get; set; } = 1.5f;

        [Description("The prefix to use when writing CASSIE messages to allow CASSIE replacer to take over.")]
        public string CustomCassiePrefix { get; set; } = "customcassie";

        /// <summary>
        /// Gets or sets the directory from which audio files are sourced.
        /// </summary>
        [Description("This is the folders where all of your audio clips will be stored. IMPORTANT: DIRECTORIES ARE ABSOLUTE, NOT RELATIVE!")]
        public List<CassieDirectorySerializable> BaseDirectories { get; set; } = new List<CassieDirectorySerializable>
        {
            new CassieDirectorySerializable(),
        };

        /// <summary>
        /// Gets or sets the volume that <see cref="Plugin.CassiePlayer"/> plays audio at.
        /// </summary>
        [Description("This is the volume of the speaker making CASSIE's words. Please adjust so that words spoken are loud enough to overpower the PA noise, but not so loud it clips or hurts to listen to.")]
        public float CassieVolume { get; set; } = 1f;

        public CassieOverrideConfigs CassieOverrideConfig { get; set; } = new CassieOverrideConfigs();
    }
}
