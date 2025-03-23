namespace CassieReplacement
{
    using Exiled.API.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class Config : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is in debug mode.
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the directory from which audio files are sourced.
        /// </summary>
        [Description("This is the folders where all of your audio clips will be stored. Defaults to EXILED/Configs if is set to null or empty. \n\nIMPORTANT: DIRECTORIES ARE ABSOLUTE, NOT RELATIVE!")]
        public List<string> BaseDirectories { get; set; } = new List<string>
        {
            "cassie",
        };

        /// <summary>
        /// Gets or sets the volume that <see cref="Plugin.CassiePlayer"/> plays audio at.
        /// </summary>
        [Description("This is the volume of the speaker making CASSIE's words. Please adjust so that words spoken are loud enough to overpower the PA noise, but not so loud it clips or hurts to listen to.")]
        public float CassieVolume { get; set; } = 5f;

        /// <summary>
        /// Gets or sets the number of seconds each clip has subtracted from its 'length'.
        /// </summary>
        [Description("This is the amount of seconds that each clip is allowed to bleed into the next one. Intended use case is for reverb.")]
        public float CassieReverb { get; set; } = 0f;
    }
}
