namespace CassieReplacement.Reader
{
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
    public class CassieAnnouncement
    {
        public static CassieAnnouncement operator +(CassieAnnouncement left, CassieAnnouncement right)
        {
            return new CassieAnnouncement($"{left.Words} {right.Words}", $"{left.Translation} {right.Translation}");
        }

        private static CassieOverrideConfigs Config => Plugin.Singleton.Config.CassieOverrideConfig;

        private static int ScpsLeft => ReferenceHub.AllHubs.Where(hub => hub.IsSCP(includeZombies: false)).Count();

        private static int PlayersLeft(Team team) => ReferenceHub.AllHubs.Where(hub => hub.GetTeam() == team).Count();

        public CassieAnnouncement Replace(string oldText, CassieAnnouncement newText)
        {
            return new CassieAnnouncement(Words.Replace(oldText, newText.Words), Translation.Replace(oldText, newText.Translation));
        }

        public CassieAnnouncement Replace(string oldText, string newText)
        {
            return new CassieAnnouncement(Words.Replace(oldText, newText), Translation.Replace(oldText, newText));
        }

        public CassieAnnouncement(string words, string translation = "")
        {
            Words = words;
            Translation = translation;
        }

        public void ReplaceVoid(string oldText, string newText)
        {
            Words = Words.Replace(oldText, newText);
            Translation = Translation.Replace(oldText, newText);
        }

        public void ReplaceVoid(string oldText, CassieAnnouncement newText)
        {
            Words = Words.Replace(oldText, newText.Words);
            Translation = Translation.Replace(oldText, newText.Translation);
        }

        /// <summary>
        /// The basic replacements to do. Run right before announcement to ensure it is up-to-date!
        /// </summary>
        /// <returns>A new <see cref="CassieAnnouncement"/> with most replacements applied.</returns>
        public CassieAnnouncement GenericReplacement()
        {
            return new CassieAnnouncement(Words, Translation)

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

        public CassieAnnouncement()
        {
        }

        private string words;

        private string translation;

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

        public void Announce(bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            CassieAnnouncement processed = GenericReplacement();
            Words = processed.Words;
            Translation = processed.Translation;

            if (string.IsNullOrWhiteSpace(Words))
            {
                return;
            }

            if (IsCustomMessage)
            {
                CustomCassieReader.Singleton.CassieReadMessage(Words, isNoisy, isSubtitles, Translation);
            }
            else
#if EXILED
            {
                if (string.IsNullOrWhiteSpace(Translation))
                {
                    Cassie.Message(Words, isHeld, isNoisy, isSubtitles);
                }
                else
                {
                    Cassie.MessageTranslated(Words, Translation, isHeld, isNoisy, isSubtitles);
                }
            }
#else
            {
                if (string.IsNullOrWhiteSpace(Translation))
                {
                    RespawnEffectsController.PlayCassieAnnouncement(Words, isHeld, isNoisy, isSubtitles);
                }
                else
                {
                    RespawnEffectsController.PlayCassieAnnouncement(MessageTranslated(Words, Translation), isHeld, isNoisy, isSubtitles);
                }
            }
#endif
        }

        /// <summary>
        /// Copied from EXILED's Cassie.MessageTranslated for LabAPI.
        /// </summary>
        public static string MessageTranslated(string message, string translation)
        {
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            string[] array = message.Split('\n');
            string[] array2 = translation.Split('\n');
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array2[i].Replace(' ', '\u2005') + "<size=0> " + array[i] + " </size><split>");
            }

            string output = stringBuilder.ToString();
            StringBuilderPool.Shared.Return(stringBuilder);
            return output;
        }
    }
}
