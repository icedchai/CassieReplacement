namespace CassieReplacement.Commands
{
    using CommandSystem;
    using System;

    /// <summary>
    /// The command used to see all registered Custom CASSIE words.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ListWordsCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "listwords";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "listwords" };

        /// <inheritdoc/>
        public string Description => "Lists all registered CUSTOMCASSIE words.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                response = $"{NineTailedFoxAnnouncer.singleton.CalculateDuration(arguments.At(0))}";
                return true;
            }

            string words = "The available words are:\n";
            foreach (string word in Plugin.RegisteredClipNames)
            {
                words += $"{word}, ";
            }

            response = words;
            return true;
        }
    }
}
