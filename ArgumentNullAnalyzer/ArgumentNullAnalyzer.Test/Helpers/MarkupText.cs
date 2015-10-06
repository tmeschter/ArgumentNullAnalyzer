using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ArgumentNullAnalyzer.Test.Helpers
{
    public sealed class MarkupText
    {
        private static readonly Regex s_regex = new Regex(@"\[\||\|\]|\$\$");

        private readonly string _text;
        private readonly ImmutableArray<LinePosition> _positions;
        private readonly ImmutableArray<LinePositionSpan> _spans;

        public MarkupText(string text)
        {
            var lines = new List<string>();

            var positionsBuilder = ImmutableArray.CreateBuilder<LinePosition>();
            var spansBuilder = ImmutableArray.CreateBuilder<LinePositionSpan>();

            var spanStartsStack = new Stack<LinePosition>();

            var reader = new StringReader(text);

            var lineNumber = 0;
            string line = null;

            while ((line = reader.ReadLine()) != null)
            {
                var lineBuilder = new StringBuilder();
                var indexInLine = 0;
                foreach (Match match in s_regex.Matches(line))
                {
                    lineBuilder.Append(line, indexInLine, match.Index - indexInLine);

                    if (match.Value == "$$")
                    {
                        positionsBuilder.Add(new LinePosition(lineNumber, lineBuilder.Length));
                    }
                    else if (match.Value == "[|")
                    {
                        spanStartsStack.Push(new LinePosition(lineNumber, lineBuilder.Length));
                    }
                    else
                    {
                        if (spanStartsStack.Count == 0)
                        {
                            var index = match.Index;
                            string contextSnippet = GetExceptionContextSnippet(line, index);
                            var exceptionMessage = $"Unmatched span end at line {lineNumber}, columm {index}: {contextSnippet}";
                            throw new ArgumentException(exceptionMessage, nameof(text));
                        }

                        var spanStart = spanStartsStack.Pop();
                        var spanEnd = new LinePosition(lineNumber, lineBuilder.Length);
                        spansBuilder.Add(new LinePositionSpan(spanStart, spanEnd));
                    }

                    indexInLine = match.Index + match.Length;
                }

                lineBuilder.Append(line, indexInLine, line.Length - indexInLine);

                lines.Add(lineBuilder.ToString());

                lineNumber = lineNumber + 1;
            }

            if (spanStartsStack.Count != 0)
            {
                var position = spanStartsStack.Peek();
                var contextSnippet = GetExceptionContextSnippet(lines[position.Line], position.Character);
                var exceptionMessage = $"Unmatched span start at line {position.Line}, column {position.Character}: {contextSnippet}";
                throw new ArgumentException(exceptionMessage, nameof(text));
            }

            _text = string.Join(Environment.NewLine, lines);

            _positions = positionsBuilder.ToImmutable();
            spansBuilder.Reverse();
            _spans = spansBuilder.ToImmutable();
        }

        public string Text => _text;
        public ImmutableArray<LinePosition> Positions => _positions;
        public ImmutableArray<LinePositionSpan> Spans => _spans;

        private static string GetExceptionContextSnippet(string text, int index)
        {
            var contextStartingIndex = Math.Max(0, index - 5);
            var contextLength = Math.Min(12, text.Length - contextStartingIndex);
            var contextSnippet = text.Substring(contextStartingIndex, contextLength);
            return contextSnippet;
        }
    }
}
