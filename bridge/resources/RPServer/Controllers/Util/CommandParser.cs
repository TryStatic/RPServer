using System;
using System.Collections.Generic;
using RPServer.Util;

namespace RPServer.Controllers.Util
{
    internal class CommandParser
    {
        private readonly Queue<string> _tokens;

        public CommandParser(string commandText)
        {
            _tokens = new Queue<string>();
            ParseCommandText(commandText);
        }

        private void ParseCommandText(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText)) return;

            var trimmed = commandText.Trim();
            var arguments = trimmed.Split(' ');

            foreach (var arg in arguments) _tokens.Enqueue(arg.ToLower());
        }

        public string GetNextToken()
        {
            return _tokens.TryDequeue(out var text) ? text : null;
        }

        public bool HasNextToken()
        {
            return _tokens.Count > 0;
        }

        public bool HasNextToken(Type type)
        {
            if (!HasNextToken()) return false;

            if (type == typeof(int))
            {
                var token = _tokens.Peek();
                var isInt = int.TryParse(token, out var result);
                return isInt;
            }

            Logger.GetInstance().ServerError("HasNextToken doesn't support " + type);
            return false;
        }
    }
}