namespace CassieReplacement.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NVorbis;

    public class CassieClip
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CassieClip"/> class.
        /// </summary>
        /// <param name="file">The FileInfo.</param>
        public CassieClip(FileInfo file)
        {
            VorbisReader vorbisReader = new (file.FullName);
            FileInfo = file;
            Name = Path.GetFileNameWithoutExtension(file.FullName).ToLower().Replace(' ', '_');
            Length = vorbisReader.TotalTime.TotalSeconds - Plugin.PluginConfig.CassieReverb > 0 ? (float)vorbisReader.TotalTime.TotalSeconds - Plugin.PluginConfig.CassieReverb : 0f;
            vorbisReader.Dispose();
        }

        public float Length { get; set; } = 0f;

        public string Name { get; set; }

        public FileInfo FileInfo { get; set; }
    }
}
