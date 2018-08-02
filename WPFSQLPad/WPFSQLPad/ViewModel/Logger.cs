using System.Text;
using System.Windows;

namespace WPFSQLPad.ViewModel
{
    /// <summary>
    /// Class responsible for writing to log.
    /// </summary>
    public class Logger
    {
        private readonly StringBuilder logBuilder;
        private readonly ILogRecipient logRecipient;
        private const string Indent = "\t";

        public Logger(ILogRecipient logRecipient)
        {
            logBuilder = new StringBuilder(256);
            this.logRecipient = logRecipient;
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

        public void Flush()
        {
            if (logRecipient != null)
            {
                logRecipient.Log = logBuilder.ToString();
            }
        }

        public void Clear()
        {
            logBuilder.Clear();
            Flush();
        }

        public void CopyToClipboard()
        {
            Clipboard.SetText(logBuilder.ToString());
            WriteLine("Log has been copied to clipboard.");
            Flush();
        }
    }
}
