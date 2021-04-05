using TextParser.Parser;
using TextParserUtils.Parser;

namespace TextInterfaceModule.Logic.Parser
{
    public class CommandParserRule : ParserRule<CommandDelegate>
    {
        public CommandParserRule(ParserSequence sequence, CommandDelegate command)
            : base(sequence, command)
        {
        }

        public CommandParserRule(ParserSequence sequence, CommandDelegate command, string example)
            : base(sequence, command, example)
        {
        }
    }
}
