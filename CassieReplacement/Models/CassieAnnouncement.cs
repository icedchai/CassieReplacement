namespace CassieReplacement.Models
{
#if EXILED
    using Exiled.API.Features;
#endif
    using NorthwoodLib.Pools;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Utf8Json.Resolvers.Internal;
    using YamlDotNet.Serialization;

#pragma warning disable SA1600
    public class CassieAnnouncement
    {

        public static CassieAnnouncement operator +(CassieAnnouncement left, CassieAnnouncement right)
        {
            return new CassieAnnouncement($"{left.Words} {right.Words}", $"{left.Translation} {right.Translation}");
        }

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
            if (string.IsNullOrWhiteSpace(Words))
            {
                return;
            }

            if (IsCustomMessage)
            {
                CustomCassieReader.Singleton.CassieReadMessage(Words, isNoisy, isSubtitles ? Translation : string.Empty);
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
        private static string MessageTranslated(string message, string translation)
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
