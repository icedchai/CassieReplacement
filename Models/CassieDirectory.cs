namespace CassieReplacement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Describes a directory serializable.
    /// </summary>
    [Serializable]
    public class CassieDirectory
    {
        [Description("Path of this directory.")]
        public string Path { get; set; } = "C:/test";

        [Description("Prefix to put in the name of each registered clip from this directory.")]
        public string Prefix { get; set; } = "";

        [Description("The amount of time each clip in this directory may bleed into the next.")]
        public float BleedTime { get; set; } = 0f;
    }
}
