using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SangoUtils.UnityDevelopToolKits
{
    public sealed class CodeWriter
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private int _indentLevel;

        public void AppendLine(string value = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                _buffer.AppendLine();
            }
            else
            {
                _buffer.AppendLine($"{new string(' ', _indentLevel * 4)} {value}");
            }
        }

        public override string ToString() => _buffer.ToString();

        public IDisposable BeginIndentScope() => new IndentScope(this);

        public IDisposable BeginBlockScope(string startLine = null, bool isReturn = false) =>
            new BlockScope(this, startLine, isReturn);

        public void IncreaseIndent()
        {
            _indentLevel++;
        }

        public void DecreaseIndent()
        {
            if (_indentLevel > 0)
                _indentLevel--;
        }

        public void BeginBlock()
        {
            AppendLine("{");
            IncreaseIndent();
        }

        public void EndBlock(bool withSemicolon = false)
        {
            DecreaseIndent();
            AppendLine(withSemicolon ? "};" : "}");
        }

        public void Clear()
        {
            _buffer.Clear();
            _indentLevel = 0;
        }

        private readonly struct IndentScope : IDisposable
        {
            private readonly CodeWriter _source;

            public IndentScope(CodeWriter source)
            {
                _source = source;
                source.IncreaseIndent();
            }

            public void Dispose()
            {
                _source.DecreaseIndent();
            }
        }

        public readonly struct BlockScope : IDisposable
        {
            private readonly CodeWriter _source;
            private readonly bool _withSemicolon;

            public BlockScope(CodeWriter source, string startLine = null, bool withSemicolon = false)
            {
                _source = source;
                _withSemicolon = withSemicolon;
                source.AppendLine(startLine);
                source.BeginBlock();
            }

            public void Dispose()
            {
                _source.EndBlock(_withSemicolon);
            }
        }
    }

    public static class Validator
    {
        public static string ValidateVariableName(string name)
        {
            var varName = "";
            var matches = Regex.Matches(name, "[A-Za-z0-9_]+");

            for (var i = 0; i < matches.Count; i++)
            {
                if (i > 0)
                    varName += "_";
                varName += matches[i].Value;
            }

            return varName;
        }
    }
}
