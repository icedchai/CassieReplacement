namespace CassieReplacement.Patches
{
#pragma warning disable
    using CassieReplacement.Models;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            // Logger.Info(words);

            // Checks for EXILED subtitle signatures.
            if (words.Contains("<size=0>") || words.Contains("<split>"))
            {
                string[] dividedBySplits = words.Split(new string[] { "</size><split>" }, StringSplitOptions.None);

                // If customcassie signature not found allow regular execution.
                // Also prevents infinite self-call
                if (!dividedBySplits[0].StartsWith(Plugin.Singleton.Config.CustomCassiePrefix))
                {
                    return true;
                }
                else
                {
                    dividedBySplits[0].Remove(0, Plugin.Singleton.Config.CustomCassiePrefix.Length);
                }

                StringBuilder subtitles = StringBuilderPool.Shared.Rent();
                StringBuilder input = StringBuilderPool.Shared.Rent();

                for (int i = 0; i < dividedBySplits.Length; i++)
                {
                    string section = dividedBySplits[i];
                    if (string.IsNullOrWhiteSpace(section))
                    {
                        continue;
                    }

                    string[] dividedBySize = section.Split(new string[] { "<size=0>" }, StringSplitOptions.None);
                    subtitles.Append(dividedBySize[0]);
                    input.Append(dividedBySize[1]);
                    if (i < dividedBySplits.Length - 2)
                    {
                        subtitles.Append("<split>");
                        input.Append("<split>");
                    }
                }

                new CassieAnnouncement(input.ToString(), subtitles.ToString()).Announce();

                // CustomCassieReader.Singleton.CassieReadMessage(input.ToString().ToLower().Split(' ').ToList(), isNoisy: makeNoise, translation: subtitles.ToString());
                StringBuilderPool.Shared.Return(input);
                StringBuilderPool.Shared.Return(subtitles);
                return false;
            }

            if (words.StartsWith(Plugin.Singleton.Config.CustomCassiePrefix))
            {
                string[] wordsplit = words.Split(';');
                List<string> input = wordsplit[0].ToLower().Split(' ').ToList();
                input.Remove(Plugin.Singleton.Config.CustomCassiePrefix);
                CustomCassieReader.Singleton.CassieReadMessage(input, makeNoise, wordsplit.Count() > 1 ? wordsplit[1] : string.Empty);
                return false;
            }

            return true;
        }

        // test\u2005subtitles<size=0>test subtitles</size><split>
        public static bool TranslatedMessagePrefix(string message, string translation, bool isHeld, bool isNoisy, bool isSubtitles)
        {
            if (message.StartsWith(Plugin.Singleton.Config.CustomCassiePrefix))
            {
                List<string> input = message.ToLower().Split(' ').ToList();
                input.Remove(Plugin.Singleton.Config.CustomCassiePrefix);
                CustomCassieReader.Singleton.CassieReadMessage(input, isNoisy, translation);
                return false;
            }

            return true;
        }
    }
}
