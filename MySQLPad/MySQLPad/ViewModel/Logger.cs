using System.Text;

namespace MySQLPad.ViewModel
{
    /// <summary>
    /// Class responsible for writing to log.
    /// </summary>
    public class Logger
    {
        private readonly StringBuilder logBuilder;
        private const string Indent = "\t";

        public Logger()
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

        public void WriteLine(string text, int indent = 0)
        {
            Write(text, indent);
            logBuilder.Append("\n");
        }

        public string Flush()
        {
            return logBuilder.ToString();
        }

        public void Clear()
        {
            logBuilder.Clear();
        }
    }
}
