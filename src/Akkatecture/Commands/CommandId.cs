using Akkatecture.Core;

namespace Akkatecture.Commands
{
    public class CommandId : Identity<CommandId>, ICommandId
    {
        public CommandId(string value) : base(value)
        {
        }
    }
}