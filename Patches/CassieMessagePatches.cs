namespace CassieReplacement.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HarmonyLib;
    using Mirror;
    using NorthwoodLib;
    using NorthwoodLib.Pools;
    using Respawning;

    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            // Checks for EXILED subtitle signatures.
            if (words.Contains("<size=0>") || words.Contains("<split>"))
            {
                string[] dividedBySplits = words.Split(new string[] { "</size><split>" }, StringSplitOptions.None);

                // If customcassie signature not found allow regular execution.
                if (!dividedBySplits[0].StartsWith("customcassie"))
                {
                    return true;
                }

                StringBuilder subtitles = StringBuilderPool.Shared.Rent();
                StringBuilder input = StringBuilderPool.Shared.Rent();

                foreach (string section in dividedBySplits)
                {
                    string[] dividedBySize = section.Split(new string[] { "<size=0>" }, StringSplitOptions.None);
                    subtitles.Append(dividedBySize[0]);
                    subtitles.Append("<split>");
                    input.Append(dividedBySize[1]);
                }

                CustomCassieReader.Singleton.CassieReadMessage(input.ToString().ToLower().Split(' ').ToList(), isNoisy: makeNoise, translation: subtitles.ToString());
                StringBuilderPool.Shared.Return(input);
                StringBuilderPool.Shared.Return(subtitles);
                return false;
            }

            if (words.StartsWith("customcassie"))
            {
                List<string> input = words.ToLower().Split(' ').ToList();
                input.Remove("customcassie");
                CustomCassieReader.Singleton.CassieReadMessage(input, makeNoise);
                return false;
            }

            return true;
        }

        // test\u2005subtitles<size=0>test subtitles</size><split>
        public static bool TranslatedMessagePrefix(string message, string translation, bool isHeld, bool isNoisy, bool isSubtitles)
        {
            if (message.StartsWith("customcassie"))
            {
                List<string> input = message.ToLower().Split(' ').ToList();
                input.Remove("customcassie");
                CustomCassieReader.Singleton.CassieReadMessage(input, isNoisy, translation);
                return false;
            }

            return true;
        }
    }
}
