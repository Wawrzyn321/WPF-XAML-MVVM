using System;
using Model.ConnectionModels;
using WPFSQLPad.ConnectionWrappers;

namespace WPFSQLPad.TreeItems
{
    public class Routine : TreeItem
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

        public Routine(string name, string type, string parameters, string returnType, DatabaseConnectionWrapper connection)
        : this(name, (RoutineType)Enum.Parse(typeof(RoutineType), type, true), parameters, returnType, connection)
        {
            
        }

        public Routine(string name, RoutineType type, string parameters, string returnType, DatabaseConnectionWrapper connection) 
            : base(connection)
        {
            Name = name;
            Type = type;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public string GetCode()
        {
            return ConnectionReference.GetRoutineCode(Type, Name);
        }

        public override string ToString()
        {
            char firstLetterOfType = Type.ToString()[0];
            return $"{ReturnType} {Name}:({Parameters}) ({firstLetterOfType})";

        }
    }
}
