namespace CassieReplacement.Commands
{
    using CommandSystem;
    using Exiled.API.Features;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The command used to calculate the duration of any given string when read out by CASSIE.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CassieCalcCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "cassiecalc";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "calc" };

        /// <inheritdoc/>
        public string Description => "Cassie calculation command";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> words = arguments.ToList();
            float dur = Cassie.CalculateDuration(string.Join(" ", words));
            response = $"Tried {string.Join(" ", words)}, got {dur} units.";
            return true;
        }
    }
}
