namespace CassieReplacement.Patches
{
    using Cassie;
#pragma warning disable
    using CassieReplacement.Reader;
    using CommandSystem.Commands.RemoteAdmin;
    using HarmonyLib;
    using LabApi.Features.Console;
    using NorthwoodLib.Pools;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [HarmonyPatch(typeof(CassieAnnouncementDispatcher), nameof(CassieAnnouncementDispatcher.AddToQueue))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(CassieAnnouncement announcement)
        {
            string words = announcement.Payload.Content;
            bool makeNoise = announcement.Payload.PlayBackground;
            bool customAnnouncement = announcement.Payload.SubtitleSource != CassieTtsPayload.SubtitleMode.None;
            string customSubtitles = announcement.Payload._customSubtitle;

            //Logger.Info($"Content: {words}, PlayBackground: {makeNoise}, SubtitleSource: {announcement.Payload.SubtitleSource}, customSubtitle: {announcement.Payload._customSubtitle}");

            bool useCassie = words.IndexOf("nocassie", StringComparison.OrdinalIgnoreCase) == -1;
            if (words.IndexOf("noparse", StringComparison.OrdinalIgnoreCase) != -1)
            {
                //Logger.Info("Allowed because noparse");
                return true;
            }

            // Checks for EXILED subtitle signatures.
            if (words.Contains("<size=0>"))
            {
                string[] dividedBySplits = words.Split(new string[] { "</size><split>" }, StringSplitOptions.None);

                // If customcassie signature not found allow regular execution.
                // Also prevents infinite self-call
                if (dividedBySplits[0].StartsWith(Plugin.Singleton.Config.CustomCassiePrefix))
                {
                    dividedBySplits[0].Remove(0, Plugin.Singleton.Config.CustomCassiePrefix.Length);
                }
                else if (Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAll)
                {
                }
                else
                {
                    return true;
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
                    input.Append(dividedBySize.TryGet(1, out string input1) ? input1 : input);
                    if (i < dividedBySplits.Length - 2)
                    {
                        subtitles.Append("<split>");
                        input.Append("<split>");
                    }
                }

                // new CassieAnnouncement(input.ToString(), subtitles.ToString()).Announce();

                CustomCassieReader.Singleton.CassieReadMessage(input.ToString().ToLower().Split(' ').ToList(), makeNoise, customAnnouncement, subtitles.ToString(), useCassie);
                StringBuilderPool.Shared.Return(input);
                StringBuilderPool.Shared.Return(subtitles);
                return false;
            }

            if (words.StartsWith(Plugin.Singleton.Config.CustomCassiePrefix) || Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAll)
            {
                string[] wordsplit = words.Split(';');
                List<string> input = wordsplit[0].ToLower().Split(' ').ToList();
                input.Remove(Plugin.Singleton.Config.CustomCassiePrefix);
                CustomCassieReader.Singleton.CassieReadMessage(input, makeNoise, customAnnouncement, wordsplit.Count() > 1 ? wordsplit[1] : customSubtitles ?? string.Empty, useCassie);
                return false;
            }

            return true;
        }
    }
}
