namespace CassieReplacement.Models
{
    using CassieReplacement.Models.Enums;
    using PlayerRoles;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

#pragma warning disable SA1600
    public class CassieOverrideConfigs
    {
        public bool ShouldOverrideAnnouncements { get; set; } = false;

        public CassieAnnouncement NtfWaveAnnouncement { get; set; } = new ("mtfunit epsilon 11 designated {letter} {number} hasentered allremaining {threatoverview}",
            "Mobile Task Force Unit Epsilon-11 designated {letter}-{number} has entered the facility.<split>All remaining personnel are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination.<split>{threatoverview}");

        public CassieAnnouncement ThreatOverviewNoScps { get; set; } = new("noscpsleft", "Substantial threat to safety remains within the facility -- exercise caution.");

        public CassieAnnouncement ThreatOverviewOneScp { get; set; } = new ("awaitingrecontaiment 1 scpsubject", "Awaiting recontaiment of: 1 SCP subject.");

        public CassieAnnouncement ThreatOverviewScps { get; set; } = new ("awaitingrecontainment {scps} scpsubjects", "Awaiting recontainment of: {scps} SCP subjects.");

        public CassieAnnouncement ScpTerminationAnnouncement { get; set; } = new ("{scp} {deathcause}", "{scp} {deathcause}");


        public Dictionary<RoleTypeId, CassieAnnouncement> ScpLookupTable { get; set; } = new Dictionary<RoleTypeId, CassieAnnouncement>
        {
            { RoleTypeId.Scp049, new CassieAnnouncement("Scp 0 4 9", "SCP-049") },
            { RoleTypeId.Scp0492, new CassieAnnouncement("Scp 0 4 9 2", "SCP-049-2") },
            { RoleTypeId.Scp096, new CassieAnnouncement("Scp 0 9 6", "SCP-096") },
            { RoleTypeId.Scp079, new CassieAnnouncement("Scp 0 7 9", "SCP-079") },
            { RoleTypeId.Scp106, new CassieAnnouncement("Scp 1 0 6", "SCP-106") },
            { RoleTypeId.Scp939, new CassieAnnouncement("Scp 9 3 9", "SCP-939") },
            { RoleTypeId.Scp3114, new CassieAnnouncement("Scp 3 1 1 4", "SCP-3114") },
        };

        public Dictionary<CassieDamageType, CassieAnnouncement> DamageTypeTerminationAnnouncementLookupTable { get; set; } = new Dictionary<CassieDamageType, CassieAnnouncement>
        {
            { CassieDamageType.Tesla, new CassieAnnouncement(" SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM", "successfully terminated by automatic security system.") },

            { CassieDamageType.Warhead, new CassieAnnouncement(" SUCCESSFULLY TERMINATED BY alpha warhead", "successfully terminated by Alpha Warhead.") },

            { CassieDamageType.Decontamination, new CassieAnnouncement(" lost in decontamination sequence", "lost in decontamination sequence.") },

            { CassieDamageType.Player, new CassieAnnouncement(" Containedsuccessfully {team}", "contained successfully {team}.") },

            { CassieDamageType.Unknown, new CassieAnnouncement(" SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", "successfully terminated. Termination cause unspecified.") },
        };

        public Dictionary<Team, CassieAnnouncement> TeamTerminationCallsignLookupTable { get; set; } = new Dictionary<Team, CassieAnnouncement>
        {
            { Team.ClassD, new CassieAnnouncement(" BY CLASSD PERSONNEL", "by Class-D personnel") },

            { Team.ChaosInsurgency, new CassieAnnouncement(" BY CHAOSINSURGENCY", "by Chaos Insurgency") },

            { Team.Scientists, new CassieAnnouncement(" BY SCIENCE PERSONNEL", "by Science Personnel") },

            { Team.FoundationForces, new CassieAnnouncement(" CONTAINMENTUNIT {letter} {number}", "-- Containment Unit {letter}-{number}") },

            { Team.OtherAlive, new CassieAnnouncement(" CONTAINMENTUNIT UNKNOWN", "-- Containment Unit Unknown") },

            { Team.SCPs, new CassieAnnouncement(" BY {scpkiller}", "by {scpkiller}") },
        };
    }
}
