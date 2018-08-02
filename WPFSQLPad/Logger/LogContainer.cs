using System.Text;

namespace Logger
{
    public class LogContainer
    {
        public string Content => logBuilder.ToString();

        private readonly StringBuilder logBuilder;
        private const string Indent = "\t";

        public LogContainer()
        {
            logBuilder = new StringBuilder(256);
        }

        public void Write(string text, int indent = 0)
        {
            for (int i = 0; i < indent; i++)
            {
                logBuilder.Append(Indent);
            }

            logBuilder.Append(text).Append("\n");
        }

        public void WriteLine(string text, int indent)
        {
            Write(text, indent);
            logBuilder.Append("\n");
        }

        public void Clear()
        {
            logBuilder.Clear();
        }
    }
}

