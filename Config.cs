namespace CassieReplacement
{
    using CassieReplacement.Models;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets the directory from which audio files are sourced.
        /// </summary>
        [Description("This is the folders where all of your audio clips will be stored. IMPORTANT: DIRECTORIES ARE ABSOLUTE, NOT RELATIVE!")]
        public List<CassieDirectory> BaseDirectories { get; set; } = new List<CassieDirectory>
        {
            new CassieDirectory()
        };

        /// <summary>
        /// Gets or sets the volume that <see cref="Plugin.CassiePlayer"/> plays audio at.
        /// </summary>
        [Description("This is the volume of the speaker making CASSIE's words. Please adjust so that words spoken are loud enough to overpower the PA noise, but not so loud it clips or hurts to listen to.")]
        public float CassieVolume { get; set; } = 1f;
    }
}
