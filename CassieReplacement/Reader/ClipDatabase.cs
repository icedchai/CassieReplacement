namespace CassieReplacement.Reader
{
    using CassieReplacement.Config;
    using CassieReplacement.Reader.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

#if EXILED
    using Exiled.API.Features;
#endif

    /// <summary>
    /// A database of clips to read from.
    /// </summary>
    public class ClipDatabase
    {
        private List<CassieClip> registeredClips { get; set; } = new List<CassieClip>();

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/>'s.
        /// </summary>
        public List<CassieClip> RegisteredClips => registeredClips;

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/> names.
        /// </summary>
        public List<string> RegisteredClipNames => RegisteredClips.Select(c => c.Name).ToList();

        /// <summary>
        /// Gets a list of all registered <see cref="CassieClip"/> names where ShouldList is set to true.
        /// </summary>
        public List<string> ListableClipNames => RegisteredClips.Where(c => c.ShouldList).Select(c => c.Name).ToList();

        /// <summary>
        /// Gets a clip with the specified name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The first <see cref="CassieClip"/> registered whose name is the same as <paramref name="name"/>, or null.</returns>
        public CassieClip GetClip(string name)
        {
            name = name.ToLower();
            IEnumerable<CassieClip> clips = registeredClips.Where(c => c.Name == name);
            return clips.FirstOrDefault();
        }

        /// <summary>
        /// Gets the length of the specified clip.
        /// </summary>
        /// <param name="clipName">The name of the clip in question.</param>
        /// <returns>A float representing how long a plugin-registered clip is, minus <see cref="Config.CassieReverb"/>, or zero.</returns>
        public float GetClipLength(string clipName)
        {
            CassieClip clip = GetClip(clipName);
            if (clip is not null)
            {
                return clip.Length;
            }

            return 0f;
        }

        /// <summary>
        /// Gets the base length of the specified clip.
        /// </summary>
        /// <param name="clipName">The name of the clip in question.</param>
        /// <returns>A float representing how long a plugin-registered clip is, or zero.</returns>
        public float GetClipBaseLength(string clipName)
        {
            CassieClip clip = GetClip(clipName);
            if (clip is not null)
            {
                return clip.BaseLength;
            }

            return 0f;
        }

        /// <summary>
        /// Registers a folder.
        /// </summary>
        /// <param name="directoryConfiguration">The Directory Serializable to use.</param>
        /// <param name="directory">Used to help with recursion.</param>
        public void RegisterFolder(CassieDirectorySerializable directoryConfiguration, string directory = null)
        {
#if EXILED
            directoryConfiguration.Path = directoryConfiguration.Path.Replace("{exiled_config}", Paths.Configs);
#endif
            DirectoryInfo d = new DirectoryInfo(directoryConfiguration.Path);
            if (directory is not null)
            {
                d = new DirectoryInfo(directory);
            }

            if (!d.Exists)
            {
                return;
            }

            foreach (DirectoryInfo directoryInfo in d.GetDirectories())
            {
                RegisterFolder(directoryConfiguration, directoryInfo.FullName);
            }

            foreach (FileInfo file in d.GetFiles("*.ogg"))
            {
                CassieClip cassieClip = new CassieClip(file, directoryConfiguration.BleedTime, directoryConfiguration.Prefix, directoryConfiguration.ShouldList);
                // Prevent duplicates from being registered by appending _ to the name as needed.
                while (RegisteredClipNames.Contains(cassieClip.Name))
                {
                    cassieClip.Name += "_";
                }

                registeredClips.Add(cassieClip);
            }
        }

        /// <summary>
        /// Unregisters all clips.
        /// </summary>
        public void UnregisterClips()
        {
            registeredClips.Clear();
        }
    }
}
