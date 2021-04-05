using TextInterfaceModule.Logic.Parser;
using TextParser.Parser;
using TextParserUtils.Parser;

namespace TextInterfaceModule.Interface
{
    public interface ITextInterfaceControl
    {
        void AddToken(string token, string regex);
        void AddRule(CommandParserRule rule);
        void AddRule(ParserSequence sequence, CommandDelegate command, string exampleDeprecated);
        void AddRule(ParserSequence sequence, CommandDelegate command);
    }
}
