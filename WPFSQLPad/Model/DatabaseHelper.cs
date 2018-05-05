using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
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

    /// <summary>
    /// Collection of helper methods for DB.
    /// </summary>
    public static class DatabaseHelper
    {
        // ' and " signify start or end of string
        private static readonly char[] escapeCharacters = { '\'', '\"' };

        //split SQL by ;
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

        //check if given string contains only characters from list (given as string)
        public static bool ContainsOnly(string value, string forbiddenCharacters)
        {
            if (value.Length == 0) return false;

            //just count "bad" chars in source string
            int count = 0;
            foreach (char c in forbiddenCharacters)
            {
                count += value.Count(charInValue => c == charInValue);
            }
            return count == value.Length;
        }

        //parse string to get query type
        public static QueryType GetQueryType(string query)
        {
            return (QueryType)Enum.Parse(typeof(QueryType), query.Substring(0, query.IndexOf(' ')), true);
        }

        //commands like SELECT, DESC and SHOW can return a table of results
        public static bool YieldsTableOutput(this QueryType queryType)
        {
            return queryType == QueryType.SELECT || queryType == QueryType.DESC || queryType == QueryType.SHOW;
        }
    }
}
