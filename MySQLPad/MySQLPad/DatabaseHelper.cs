using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace WPFPad
{
    public enum QueryType
    {
        UNKNOWN,
        SELECT,
        UPDATE,
        DELETE,
        CREATE,
        DESC,
        ALTER,
        DROP,
        SHOW,
    }

    public static class DatabaseHelper
    {
        private static readonly char[] escapeCharacters = { '\'', '\"' };

        public static List<string> SplitSqlExpression(string sql)
        {
            List<string> statements = new List<string>();
            if (string.IsNullOrEmpty(sql))
            {
                return statements;
            }

            int currentPosition = 0;
            int previousPosition = 0;
            char? currentEscape = null;
            for (; currentPosition < sql.Length; currentPosition++)
            {
                if (sql[currentPosition] == ';')
                {
                    if (currentEscape.HasValue == false)
                    {
                        string newString = sql.Substring(previousPosition, currentPosition - previousPosition).Trim(' ', ';');
                        if (newString != String.Empty && !ContainsOnly(newString, " ;"))
                        {
                            statements.Add(newString);
                        }
                        previousPosition = currentPosition;
                    }
                }
                else
                {
                    foreach (char escapeCharacter in escapeCharacters)
                    {
                        if (sql[currentPosition] == escapeCharacter)
                        {
                            if (currentEscape.HasValue == false)
                            {
                                currentEscape = escapeCharacter;
                            }
                            else
                            {
                                if (currentEscape == escapeCharacter)
                                {
                                    currentEscape = null;
                                }
                                else
                                {
                                    throw new Exception($"PARSE BRZYDKO {previousPosition} {currentPosition} {currentEscape} ({escapeCharacter})");
                                }
                            }
                        }
                    }
                }
            }

            if (currentPosition != previousPosition)
            {
                string s = sql.Substring(previousPosition, currentPosition - previousPosition).Trim(' ', ';');
                if (!string.IsNullOrEmpty(s))
                {
                    statements.Add(s);
                }
            }

            return statements;
        }

        public static bool ContainsOnly(string value, string forbiddenCharacters)
        {
            if (value.Length == 0) return false;

            int count = 0;
            foreach (char c in forbiddenCharacters)
            {
                count += value.Count(charInValue => c == charInValue);
            }
            return count == value.Length;
        }

        public static QueryType GetQueryType(string query)
        {
            return (QueryType)Enum.Parse(typeof(QueryType),query.Substring(0, query.IndexOf(' ')), true);
        }

    }
}
