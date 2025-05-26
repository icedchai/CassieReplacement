# CassieReplacement
 Plugin to modify or replace CASSIE.

Dependencies:
[AudioPlayerApi by Killers0992](https://github.com/Killers0992/AudioPlayerApi/)


https://github.com/user-attachments/assets/4a8e1ef7-ac78-451e-895a-018805555865

## How to use
1. Create a folder with `.ogg` files that are `mono` & `48000 hz` sample rate

2. In the `config.yml`, you need to add the path to the folder here:

   2.1. The `prefix` property defines how you can refer to the clip in CASSIE.
```
is_enabled: true
debug: false
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
