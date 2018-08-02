using System;

namespace WPFSQLPad.TreeItems
{
    public sealed class Routine
    {
        public enum RoutineType
        {
            Function,
            Procedure,
        }

        public string Name { get; }
        public RoutineType Type { get; }
        public string Parameters { get; }
        public string ReturnType { get; }
        public string Code { get; }

        public Routine(string name, string type, string parameters, string returnType, string code)
        : this(name, (RoutineType)Enum.Parse(typeof(RoutineType), type, true), parameters, returnType, code)
        {
            
        }

        public Routine(string name, RoutineType type, string parameters, string returnType, string code) 
        {
            Name = name;
            Type = type;
            Parameters = parameters;
            ReturnType = returnType;
            Code = code;
        }

        public override string ToString()
        {
            char firstLetterOfType = Type.ToString()[0];
            return $"{ReturnType} {Name}:({Parameters}) ({firstLetterOfType})";

        }
    }
}
