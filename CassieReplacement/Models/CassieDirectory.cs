namespace CassieReplacement.Models
{
    using System;

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
