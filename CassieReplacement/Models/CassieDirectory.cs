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
        public string Path { get; set; } = "C:/test";

        public string Prefix { get; set; } = string.Empty;

        public float BleedTime { get; set; } = 0f;
    }
}
