using System;

namespace ChatModule
{
    public class ChatMessage
    {
        public ChatMessage(string text, string signature, DateTime date)
        {
            Text = text;
            Signature = signature;
            Date = date;
        }

        public string Text { get; }
        public string Signature { get; }
        public DateTime Date { get; }

        public override string ToString()
        {
            return Date + " " + Signature + ": " + Text;
        }
    }
}