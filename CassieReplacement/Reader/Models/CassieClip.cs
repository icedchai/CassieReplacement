namespace CassieReplacement.Reader.Models
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
        public CassieClip(FileInfo file, float reverb = 0f, string prefix = "", bool shouldList = false)
        {
            VorbisReader vorbisReader = new(file.FullName);
            FileInfo = file;
            Reverb = reverb;
            Name = Path.GetFileNameWithoutExtension(file.FullName).ToLower().Replace(' ', '_');
            Name = $"{prefix}{Name}";
            BaseLength = (float)vorbisReader.TotalTime.TotalSeconds;
            ShouldList = shouldList;
            vorbisReader.Dispose();
        }

        public CassieClip(string name, FileInfo fileInfo, float baseLength, float reverb = 0f, bool shouldList = false)
        {
            Reverb = reverb;
            BaseLength = baseLength;
            Name = name;
            FileInfo = fileInfo;
            ShouldList = shouldList;
        }

        public bool ShouldList { get; set; } = false;

        public float Reverb { get; set; } = 0f;

        public float BaseLength { get; set; }

        public float Length => BaseLength - Reverb > 0 ? BaseLength - Reverb : 0f;

        public string Name { get; set; }

        public FileInfo FileInfo { get; set; }
    }
}
