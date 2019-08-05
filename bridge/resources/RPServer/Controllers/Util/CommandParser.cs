using System;
using System.Collections.Generic;

namespace RPServer.Controllers.Util
{
    internal class CommandParser
    {
        private readonly Queue<string> _tokens;

        public CommandParser(string commandText)
        {
            _tokens = new Queue<string>();

            var trimmed = commandText.Trim();
            var arguments = trimmed.Split(' ');

            foreach (var arg in arguments)
            {
                _tokens.Enqueue(arg.ToLower());
            }
        }

        public string GetNextToken() => _tokens.TryDequeue(out var text) ? text : null;

        public bool HasNextToken() => _tokens.Count > 0;
    }
}
