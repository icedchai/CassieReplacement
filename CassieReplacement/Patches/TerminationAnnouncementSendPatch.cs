#if !EXILED
namespace CassieReplacement.Patches
{
    using HarmonyLib;
    using PlayerRoles;
    using PlayerStatsSystem;

    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    public static class TerminationAnnouncementSendPatch
    {
        public static bool Prefix(ReferenceHub scp, DamageHandlerBase hit)
        {
            if (!Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAnnouncements)
            {
                return true;
            }

            if (!scp.IsSCP(includeZombies: false))
            {
                return true;
            }

            CassieEventHandlers.HandleAnnouncingTermination(hit, scp.GetRoleId());

            return false;
        }
    }
}
#endif