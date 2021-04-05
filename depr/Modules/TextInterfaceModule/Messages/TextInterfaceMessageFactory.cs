namespace TextInterfaceModule
{
    public class TextInterfaceMessageFactory
    {
        private static TextInterfaceMessageFactory _instance;

        private TextInterfaceMessageFactory() { }

        public static TextInterfaceMessageFactory GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TextInterfaceMessageFactory();
            }
            return _instance;
        }

    }
}
