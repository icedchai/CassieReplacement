namespace CassieReplacement.Config
{
    using CassieReplacement.Reader;

    /// <summary>
    /// Serializable version of <see cref="CustomCassieAnnouncement"/>.
    /// </summary>
    public class SerializableCassieAnnouncement
    {
        public SerializableCassieAnnouncement()
        {
        }

        public SerializableCassieAnnouncement(string words, string translation)
        {
            Words = words;
            Translation = translation;
        }

        public bool IsNoisy { get; set; } = true;

        public string Words { get; set; }

        public string Translation { get; set; }
    }
}
