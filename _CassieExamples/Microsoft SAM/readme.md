# This is an example/demo you can use to quickly set this up on your own server. (EXILED ONLY)

## How to install
- Download `_sam.zip`
- Extract it into your `EXILED/Configs` folder, where the audio files are inside `EXILED/Configs/_sam`
- Copy `config.yml` into your own config for this plugin.

## Explanation
[Basic explanation of the config can be found here.](https://github.com/icedchai/CassieReplacement?tab=readme-ov-file#overrides-exiled-only)

### Entrance announcement override

[Relevant config
](https://github.com/icedchai/CassieReplacement/tree/main/_CassieExamples/Microsoft%20SAM#entrance-announcement-config)
https://github.com/user-attachments/assets/5ba85417-cc29-49ae-bb2b-2297d3a78893

https://github.com/user-attachments/assets/d4bb2dce-3ba2-4115-8668-01b209cb78dc

The plugin has the capability to override all arrival announcements, like here.

### Termination announcement override

[Relevant config
](https://github.com/icedchai/CassieReplacement/tree/main/_CassieExamples/Microsoft%20SAM#termination-announcement-config)
https://github.com/user-attachments/assets/7906cdf4-d27b-402f-8805-e571120aa867

https://github.com/user-attachments/assets/3563bcf3-5076-406e-a1ff-d565e24ce249

The plugin also has the capability to override termination announcements.



### Entrance Announcement Config
```
  ntf_wave_announcement:
    words: 'mtfunit epsilon 11 designated {letter} {number} hasentered allremaining %threatoverview%'
    translation: 'Mobile Task Force Unit Epsilon-11 designated {letter}-{number} has entered the facility.<split>All remaining personnel are advised to proceed with standard evacuation protocols until an MTF squad reaches your destination.<split>{threatoverview}'
  threat_overview_no_scps:
    words: 'noscpsleft'
    translation: 'Substantial threat to safety remains within the facility -- exercise caution.'
  threat_overview_one_scp:
    words: 'awaitingrecontainment 1 scpsubject'
    translation: 'Awaiting recontaiment of: 1 SCP subject.'
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
```


### Termination Announcement Config
```
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

