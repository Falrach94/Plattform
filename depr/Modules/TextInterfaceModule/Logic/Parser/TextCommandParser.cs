using TextParser.Tokenizer;
using TextParserUtils.Parser;

namespace TextInterfaceModule
{
    public class TextCommandParser : CommandParser
    {
        public TextCommandParser() : base(new Tokenizer<string>(false))
        {

        }
    }
}
