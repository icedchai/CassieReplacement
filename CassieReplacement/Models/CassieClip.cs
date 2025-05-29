namespace CassieReplacement.Models
{
    using NVorbis;
    using System.IO;

    public class CassieClip
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CassieClip"/> class.
        /// </summary>
        /// <param name="file">The FileInfo.</param>
        /// <param name="reverb">The amount to subtract from the length of the clip.</param>
        /// <param name="prefix">The prefix to add to the name.</param>
        public CassieClip(FileInfo file, float reverb = 0f, string prefix = "")
        {
            VorbisReader vorbisReader = new(file.FullName);
            FileInfo = file;
            Name = Path.GetFileNameWithoutExtension(file.FullName).ToLower().Replace(' ', '_');
            Name = $"{prefix}{Name}";
            BaseLength = (float)vorbisReader.TotalTime.TotalSeconds;
            Length = BaseLength - reverb > 0 ? BaseLength - reverb : 0f;
            vorbisReader.Dispose();
        }

        public float BaseLength { get; set; }

        public float Length { get; set; }

        public string Name { get; set; }

        public FileInfo FileInfo { get; set; }
    }
}
