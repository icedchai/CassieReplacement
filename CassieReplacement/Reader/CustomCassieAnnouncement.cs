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

        public CustomCassieAnnouncement Replace(string oldText, CustomCassieAnnouncement newText)
        {
            return new CustomCassieAnnouncement(Words.Replace(oldText, newText.Words), Translation.Replace(oldText, newText.Translation));
        }

        public CustomCassieAnnouncement Replace(string oldText, string newText)
        {
            return new CustomCassieAnnouncement(Words.Replace(oldText, newText), Translation.Replace(oldText, newText));
        }

        public CustomCassieAnnouncement(string words, string translation = "")
        {
            Words = words;
            Translation = translation;
        }

        public void ReplaceVoid(string oldText, string newText)
        {
            Words = Words.Replace(oldText, newText);
            Translation = Translation.Replace(oldText, newText);
        }

        public void ReplaceVoid(string oldText, CustomCassieAnnouncement newText)
        {
            Words = Words.Replace(oldText, newText.Words);
            Translation = Translation.Replace(oldText, newText.Translation);
        }

        /// <summary>
        /// The basic replacements to do. Run right before announcement to ensure it is up-to-date!
        /// </summary>
        /// <returns>A new <see cref="CustomCassieAnnouncement"/> with most replacements applied.</returns>
        public CustomCassieAnnouncement GenericReplacement()
        {
            return new CustomCassieAnnouncement(Words, Translation)

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

        private string words;

        private string translation;

        public bool IsNoisy { get; set; } = true;

        public string Words
        {
            get => words;
            set => words = value.ToLower();
        }

        public string Translation
        {
            get => translation;
            set => translation = value;
        }

        [YamlIgnore]
        public bool IsCustomMessage => Words.StartsWith(Plugin.Singleton.Config.CustomCassiePrefix);

        public void Announce(bool isHeld = false, bool? isNoisy = null, bool isSubtitles = true)
        {
            bool playNoise = IsNoisy;
            if (isNoisy != null)
            {
                playNoise = !(bool)isNoisy;
            }

            CustomCassieAnnouncement processed = GenericReplacement();
            Words = processed.Words;
            Translation = processed.Translation;

            if (string.IsNullOrWhiteSpace(Words))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(Translation))
            {
                new CassieAnnouncement(new CassieTtsPayload(Words, playNoise)).AddToQueue();
            }
            else
            {
                new CassieAnnouncement(new CassieTtsPayload(Words, Translation, playNoise)).AddToQueue();
            }
        }
    }
}
