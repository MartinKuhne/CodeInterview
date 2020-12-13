using System.Collections.Generic;
using System.Linq;

namespace Rover.Lib
{
    public static class RoverCommandHelper
    {
        /// <summary>
        /// Translates character into RoverCommand
        /// </summary>
        private static readonly Dictionary<char, RoverCommand> RoverCommandMap = new Dictionary<char, RoverCommand>
        {
            { 'M', RoverCommand.Move },
            { 'L', RoverCommand.Left },
            { 'R', RoverCommand.Right }
        };

        public static List<RoverCommand> AsRoverCommand(this string commandSequence)
        {
            return commandSequence.ToCharArray().Select(c => c.AsRoverCommand()).ToList();
        }

        public static RoverCommand AsRoverCommand(this char commandCharacter)
        {
            return RoverCommandMap[commandCharacter];
        }
    }
}
