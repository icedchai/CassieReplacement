# CassieReplacement
 Plugin to modify or replace CASSIE.

Dependencies:
[AudioPlayerApi by Killers0992](https://github.com/Killers0992/AudioPlayerApi/)


https://github.com/user-attachments/assets/4a8e1ef7-ac78-451e-895a-018805555865

## How to use
1. Create a folder with `.ogg` files that are `mono` & `48000 hz` sample rate

2. In the `config.yml`, you need to add the path to the folder here:

   2.1. The `prefix` property defines how you can refer to the clip in CASSIE.
	 
	 2.2. (EXILED ONLY PAST v1.5.0) You can also define CASSIE announcement overrides for SCP termination, NTF and CI arrivals, etc. (SEE [**Overrides**](https://github.com/icedchai/CassieReplacement/blob/dev/README.md#overrides-exiled-only) in the readme. See )
```
# This is the folders where all of your audio clips will be stored. IMPORTANT: DIRECTORIES ARE ABSOLUTE, NOT RELATIVE!
base_directories:
-
# Path of this directory.
  path: 'PATH/TO/FOLDER/WITH/SOUND/FILES'
  # Prefix to put in the name of each registered clip from this directory.
  prefix: ''
  # The amount of time each clip in this directory may bleed into the next.
  bleed_time: 0
```
 
3. When writing a CASSIE announcement, type `customcassie` at the beginning to indicate to the plugin you want to use custom clips

   3.1. The filename, minus the extension, will be the name of the clip.
example: `cassie customcassie customword1 customword2`

   3.2. A silent cassie announcement will simply play the sound effects in order.
	 
	 3.3. When writing a CASSIE announcement in RA, type `;` after the words to indicate you wish to write the translation/subtitles now.

## Overrides (EXILED only)

This is the default override config.
Most word replacements/Keywords are context-specific.

### NTF Announcement Keywords

`{letter}` & `{number}` are the relevant NATO Unit (eg. FOXTROT-18). "Relevant" meaning the Unit which spawned or the Unit which killed the SCP.

`{scps}` is the number of SCPs left as reported by the `AnnouncingNtfEntranceEventArgs`


### Death Announcement Keywords

`{scp}` is the name of the SCP as defined in `scp_lookup_table`.

`{deathcause}` is the phrase associated with the specific `DamageType` as defined in `damage_type_termination_announcement_lookup_table`.

`{team}` is the phrase associated with the player who killed the SCP as defined in `team_termination_callsign_lookup_table`.

`{scpkiller}` is the name of the SCP who killed the SCP as defined in `damage_type_termination_announcement_lookup_table`. (This should not be relevant most of the time, but can happen if an SCP uses a gun to kill another SCP.)
```
cassie_override_config:
# Whether to override these CASSIE messages. Put the prefix in front to play customcassie messages.
  should_override_announcements: false
  ntf_wave_announcement:
    words: 'mtfunit epsilon 11 designated {letter} {number} hasentered allremaining {threatoverview}'
    translation: 'Mobile Task Force Unit Epsilon-11 designated {letter}-{number} has entered the facility.<split>All remaining personnel are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination.<split>{threatoverview}'
  threat_overview_no_scps:
    words: 'noscpsleft'
    translation: 'Substantial threat to safety remains within the facility -- exercise caution.'
  threat_overview_one_scp:
    words: 'awaitingrecontainment 1 scpsubject'
    translation: 'Awaiting recontainment of: 1 SCP subject.'
  threat_overview_scps:
    words: 'awaitingrecontainment {scps} scpsubjects'
    translation: 'Awaiting recontainment of: {scps} SCP subjects.'
  ntf_mini_announcement:
    words: 'ninetailedfox backup unit designated {letter} {number} hasentered {threatoverview}'
    translation: 'Nine-Tailed Fox Backup Unit designated {letter}-{number} has entered the facility.<split>{threatoverview}'
  chaos_wave_announcement:
    words: 'security alert . substantial chaos insurgent activity detected . security personnel proceed with standard protocols'
    translation: 'Security alert. Substantial Chaos Insurgent activity detected.<split>Security personnel, proceed with standard protocols.'
  chaos_mini_announcement:
    words: 'attention security personnel . chaosinsurgency spotted at gate a'
    translation: 'Attention security personnel. Chaos Insurgency spotted at Gate A.'
  scp_termination_announcement:
    words: '{scp} {deathcause}'
    translation: '{scp} {deathcause}'
  scp_lookup_table:
    Scp049:
      words: 'scp 0 4 9'
      translation: 'SCP-049'
    Scp0492:
      words: 'scp 0 4 9 2'
      translation: 'SCP-049-2'
    Scp096:
      words: 'scp 0 9 6'
      translation: 'SCP-096'
    Scp079:
      words: 'scp 0 7 9'
      translation: 'SCP-079'
    Scp106:
      words: 'scp 1 0 6'
      translation: 'SCP-106'
    Scp939:
      words: 'scp 9 3 9'
      translation: 'SCP-939'
    Scp3114:
      words: 'scp 3 1 1 4'
      translation: 'SCP-3114'
  damage_type_termination_announcement_lookup_table:
    Tesla:
      words: ' successfully terminated by automatic security system'
      translation: 'successfully terminated by automatic security system.'
    Warhead:
      words: ' successfully terminated by alpha warhead'
      translation: 'successfully terminated by Alpha Warhead.'
    Decontamination:
      words: ' lost in decontamination sequence'
      translation: 'lost in decontamination sequence.'
    Player:
      words: ' containedsuccessfully {team}'
      translation: 'contained successfully {team}.'
    Unknown:
      words: ' successfully terminated . termination cause unspecified'
      translation: 'successfully terminated. Termination cause unspecified.'
  team_termination_callsign_lookup_table:
    ClassD:
      words: ' by classd personnel'
      translation: 'by Class-D personnel'
    ChaosInsurgency:
      words: ' by chaosinsurgency'
      translation: 'by Chaos Insurgency'
    Scientists:
      words: ' by science personnel'
      translation: 'by Science Personnel'
    FoundationForces:
      words: ' containmentunit {letter} {number}'
      translation: '-- Containment Unit {letter}-{number}'
    OtherAlive:
      words: ' containmentunit unknown'
      translation: '-- Containment Unit Unknown'
    SCPs:
      words: ' by {scpkiller}'
      translation: 'by {scpkiller}'
```
