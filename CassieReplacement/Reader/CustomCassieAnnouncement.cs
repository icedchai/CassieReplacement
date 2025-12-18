namespace CassieReplacement.Reader
{
    using Cassie;
    using CassieReplacement;
    using CassieReplacement.Config;
#if EXILED
    using Exiled.API.Features;
#endif
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using Respawning;
    using System.Linq;
    using System.Text;
    using YamlDotNet.Serialization;

#pragma warning disable SA1600
    public class CustomCassieAnnouncement
    {
        public static CustomCassieAnnouncement operator +(CustomCassieAnnouncement left, CustomCassieAnnouncement right)
        {
            return new CustomCassieAnnouncement($"{left.Words} {right.Words}", $"{left.Translation} {right.Translation}");
        }

        private static CassieOverrideConfigs Config => Plugin.Singleton.Config.CassieOverrideConfig;

        private static int ScpsLeft => ReferenceHub.AllHubs.Where(hub => hub.IsSCP(includeZombies: false)).Count();

        private static int PlayersLeft(Team team) => ReferenceHub.AllHubs.Where(hub => hub.GetTeam() == team).Count();

        public CustomCassieAnnouncement Replace(string oldText, SerializableCassieAnnouncement? newText)
        {
            if (newText == null)
            {
                Words = Words.Replace(oldText, string.Empty);
                Translation = Translation.Replace(oldText, string.Empty);
            }
            else
            {
                Words = Words.Replace(oldText, newText.Words);
                Translation = Translation?.Replace(oldText, newText.Translation);
            }

            return this;
        }

        public CustomCassieAnnouncement Replace(string oldText, string newText)
        {
            Words = Words.Replace(oldText, newText);
            Translation = Translation?.Replace(oldText, newText);
            return this;
        }

        public CustomCassieAnnouncement(string words, string translation = "")
        {
            Words = StringBuilderPool.Shared.Rent(words);
            Translation = string.IsNullOrWhiteSpace(translation) ? null : StringBuilderPool.Shared.Rent(translation);
        }

        /// <summary>
        /// Performs several basic replacement operations on this instance, including threat overviews, and number of members of specific teams. Run right before announcement to ensure it is up-to-date.
        /// </summary>
        /// <returns>The same <see cref="CustomCassieAnnouncement"/> with most replacements applied.</returns>
        public CustomCassieAnnouncement PerformGenericReplacements()
        {
            return this

                // more complex ideas (needs to go first).
                .Replace("{threatoverview}", ScpsLeft == 0 ? Config.ThreatOverviewNoScps : ScpsLeft == 1 ? Config.ThreatOverviewOneScp : Config.ThreatOverviewScps)

                // pure numbers.
                .Replace("{scps}", ScpsLeft.ToString())
                .Replace("{classds}", PlayersLeft(Team.ClassD).ToString())
                .Replace("{scientists}", PlayersLeft(Team.Scientists).ToString())
                .Replace("{foundationforces}", PlayersLeft(Team.FoundationForces).ToString())
                .Replace("{chaosinsurgencys}", PlayersLeft(Team.ChaosInsurgency).ToString())
                .Replace("{flamingos}", PlayersLeft(Team.Flamingos).ToString());
        }

        public CustomCassieAnnouncement()
        {
        }

        public CustomCassieAnnouncement(SerializableCassieAnnouncement serializable)
            : this(serializable.Words, serializable.Translation)
        {
            IsNoisy = serializable.IsNoisy;
        }

        public bool IsNoisy { get; set; } = true;

        public StringBuilder Words { get; set; }

        #nullable enable

        public StringBuilder? Translation { get; set; }

        public void Announce(bool isHeld = false, bool? isNoisy = null, bool isSubtitles = true)
        {
            bool playNoise = IsNoisy;
            if (isNoisy != null)
            {
                playNoise = !(bool)isNoisy;
            }

            PerformGenericReplacements();

            if (Words == null)
            {
                return;
            }

            if (Translation == null)
            {
                new CassieAnnouncement(new CassieTtsPayload(StringBuilderPool.Shared.ToStringReturn(Words), playNoise), glitchScale: 0).AddToQueue();
            }
            else
            {
                new CassieAnnouncement(new CassieTtsPayload(StringBuilderPool.Shared.ToStringReturn(Words), StringBuilderPool.Shared.ToStringReturn(Translation), playNoise), glitchScale: 0).AddToQueue();
            }
        }
    }
}
