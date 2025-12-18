namespace CassieReplacement.Config
{
    using CassieReplacement.Reader;
    using CassieReplacement.Reader.Enums;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.ComponentModel;

#pragma warning disable SA1600
    public class CassieOverrideConfigs
    {
        [Description("Whether to override these CASSIE messages. Put the prefix in front to play customcassie messages.")]
        public bool ShouldOverrideAnnouncements { get; set; } = false;

        [Description("Whether to apply Custom CASSIE to every announcement that plays (including base-game, commands, etc). Careful with this one!")]
        public bool ShouldOverrideAll { get; set; } = false;

        public CustomCassieAnnouncement NtfWaveAnnouncement { get; set; } = new("mtfunit epsilon 11 designated {letter} {number} hasentered allremaining {threatoverview}",
            "Mobile Task Force Unit Epsilon-11 designated {letter}-{number} has entered the facility.<split>All remaining personnel are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination.<split>{threatoverview}");

        public CustomCassieAnnouncement ThreatOverviewNoScps { get; set; } = new("noscpsleft", "Substantial threat to safety remains within the facility -- exercise caution.");

        public CustomCassieAnnouncement ThreatOverviewOneScp { get; set; } = new("awaitingrecontainment 1 scpsubject", "Awaiting recontainment of: 1 SCP subject.");

        public CustomCassieAnnouncement ThreatOverviewScps { get; set; } = new("awaitingrecontainment {scps} scpsubjects", "Awaiting recontainment of: {scps} SCP subjects.");

        public CustomCassieAnnouncement NtfMiniAnnouncement { get; set; } = new("NINETAILEDFOX BACKUP UNIT designated {letter} {number} hasentered {threatoverview}",
            "Nine-Tailed Fox Backup Unit designated {letter}-{number} has entered the facility.<split>{threatoverview}");

        public CustomCassieAnnouncement ChaosWaveAnnouncement { get; set; } = new("Security Alert . Substantial Chaos Insurgent Activity Detected . Security Personnel Proceed with Standard Protocols",
            "Security alert. Substantial Chaos Insurgent activity detected.<split>Security personnel, proceed with standard protocols.");

        public CustomCassieAnnouncement ChaosMiniAnnouncement { get; set; } = new("ATTENTION SECURITY PERSONNEL . CHAOSINSURGENCY SPOTTED AT GATE A",
            "Attention security personnel. Chaos Insurgency spotted at Gate A.");

        public CustomCassieAnnouncement ScpTerminationAnnouncement { get; set; } = new("{scp} {deathcause}", "{scp} {deathcause}");

        public Dictionary<RoleTypeId, CustomCassieAnnouncement> ScpLookupTable { get; set; } = new Dictionary<RoleTypeId, CustomCassieAnnouncement>
        {
            { RoleTypeId.Scp049, new CustomCassieAnnouncement("Scp 0 4 9", "SCP-049") },
            { RoleTypeId.Scp0492, new CustomCassieAnnouncement("Scp 0 4 9 2", "SCP-049-2") },
            { RoleTypeId.Scp096, new CustomCassieAnnouncement("Scp 0 9 6", "SCP-096") },
            { RoleTypeId.Scp079, new CustomCassieAnnouncement("Scp 0 7 9", "SCP-079") },
            { RoleTypeId.Scp106, new CustomCassieAnnouncement("Scp 1 0 6", "SCP-106") },
            { RoleTypeId.Scp939, new CustomCassieAnnouncement("Scp 9 3 9", "SCP-939") },
            { RoleTypeId.Scp3114, new CustomCassieAnnouncement("Scp 3 1 1 4", "SCP-3114") },
        };

        public Dictionary<CassieDamageType, CustomCassieAnnouncement> DamageTypeTerminationAnnouncementLookupTable { get; set; } = new Dictionary<CassieDamageType, CustomCassieAnnouncement>
        {
            { CassieDamageType.Tesla, new CustomCassieAnnouncement(" SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM", "successfully terminated by automatic security system.") },

            { CassieDamageType.Warhead, new CustomCassieAnnouncement(" SUCCESSFULLY TERMINATED BY alpha warhead", "successfully terminated by Alpha Warhead.") },

            { CassieDamageType.Decontamination, new CustomCassieAnnouncement(" lost in decontamination sequence", "lost in decontamination sequence.") },

            { CassieDamageType.Player, new CustomCassieAnnouncement(" Containedsuccessfully {team}", "contained successfully {team}.") },

            { CassieDamageType.Unknown, new CustomCassieAnnouncement(" SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", "successfully terminated. Termination cause unspecified.") },
        };

        public Dictionary<Team, CustomCassieAnnouncement> TeamTerminationCallsignLookupTable { get; set; } = new Dictionary<Team, CustomCassieAnnouncement>
        {
            { Team.ClassD, new CustomCassieAnnouncement(" BY CLASSD PERSONNEL", "by Class-D personnel") },

            { Team.ChaosInsurgency, new CustomCassieAnnouncement(" BY CHAOSINSURGENCY", "by Chaos Insurgency") },

            { Team.Scientists, new CustomCassieAnnouncement(" BY SCIENCE PERSONNEL", "by Science Personnel") },

            { Team.FoundationForces, new CustomCassieAnnouncement(" CONTAINMENTUNIT {letter} {number}", "-- Containment Unit {letter}-{number}") },

            { Team.OtherAlive, new CustomCassieAnnouncement(" BY UNKNOWN PERSONNEL", "by unknown personnel") },

            { Team.SCPs, new CustomCassieAnnouncement(" BY {scpkiller}", "by {scpkiller}") },
        };
    }
}
