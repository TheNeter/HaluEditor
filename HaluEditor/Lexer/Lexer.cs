using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ngprojects.HaluEditor.Lexer
{
    //TODO maybe replace by assemblyref://Antlr4.Runtime.Standard
    public class Lexer
    {
        private static readonly Regex _regexIncludeReturn = new Regex(@"(\n|\r|(\r\n))+", RegexOptions.Compiled);
        private static readonly Regex _regexWhitespaceAndReturn = new Regex(@"((\t|\v|\f| |\u2022)*(?<NewLine>((\r\n)|(\u00B6\u00AC)|\r|\n|\u00AC|\u00B6)+)?)+", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
        private List<TokenDefinition> _definitions = new List<TokenDefinition>();

        public Lexer()
        {
        }

        public Lexer(IEnumerable<TokenDefinition> definitions)
        {
            foreach (var def in definitions)
            {
                AddDefinition(def);
            }
        }

        public List<TokenDefinition> Definitions
        {
            get => _definitions; set { _definitions = value.FindAll(x => x != null); }
        }

        /// <summary>
        /// Adds a <see cref="TokenDefinition" /> to this <see cref="Lexer" />.
        /// <para> Definitions added first will have a higher priority then tokens added later. </para>
        /// </summary>
        /// <param name="definition"> The <see cref="TokenDefinition" /> to add. </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public void AddDefinition(TokenDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            Definitions.Add(definition);
        }

        /// <summary>
        /// Generates a stream of tokens from a source input.
        /// </summary>
        /// <param name="source">                            The source that should be tokenized. </param>
        /// <param name="ignoreWhitespace">                 
        /// Determines whether or not whitespace tokens are ignored.
        /// </param>
        /// ///
        /// <param name="throwExceptionOnUnrecognizedToken">
        /// Determines whether or not to throw Exceptions on unrecognized <see cref="TokenDefinition" />
        /// </param>
        /// <returns> </returns>
        public IEnumerable<Token> Tokenize(string source, bool ignoreWhitespace = true, bool throwExceptionOnUnrecognizedToken = false)
        {
            int index = 0;
            int line = 0;
            int column = 0;

            while (index < source.Length)
            {
                int matchLength = 0;

                var whitespaceBeforeMatch = _regexWhitespaceAndReturn.Match(source, index + matchLength);
                var newLinesBeforeMatch = whitespaceBeforeMatch.Groups["NewLine"];

                if (whitespaceBeforeMatch.Success && whitespaceBeforeMatch.Length > 0)
                {
                    if (!ignoreWhitespace)
                    {
                        /* TODO write new lines in Token (replace -10000 with value)
                         * var enterInWhitespaceAM = _includeReturn.Matches(source, index + matchLength);
                         * int enterLengthInWhitespaceAM = GetResultLengthFromRegexCollention(enterInWhitespaceAM);
                         */
                        if (newLinesBeforeMatch.Success)
                        {
                            yield return new Token(TokenType.ENTER, null, whitespaceBeforeMatch.Value,
                                new TokenPosition(whitespaceBeforeMatch.Index, line, column, -10000));
                        }
                        else
                        {
                            yield return new Token(TokenType.WHITESPACE, null, whitespaceBeforeMatch.Value,
                                new TokenPosition(whitespaceBeforeMatch.Index, line, column, -10000));
                        }
                    }

                    matchLength += whitespaceBeforeMatch.Length;

                    if (newLinesBeforeMatch.Success)
                    {
                        line += newLinesBeforeMatch.Captures.Count;
                        column = whitespaceBeforeMatch.Length - (whitespaceBeforeMatch.Value.LastIndexOf(newLinesBeforeMatch.Value) + 1);
                    }
                }
                index += matchLength;
                matchLength = 0;

                TokenDefinition matchedDefinition = null;

                foreach (var rule in Definitions)
                {
                    var match = rule.Regex.Match(source, index);

                    if (match.Success && match.Index - index == 0 && match.Length > matchLength)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if (matchedDefinition == null)
                {
                    if (throwExceptionOnUnrecognizedToken)
                    {
                        throw new UnrecognizedTokenException(source[index], new TokenPosition(index, line, column, -1),
                          $"Unrecognized symbol '{source[index]}' at index {index} (line {line}, column {column})");
                    }
                    else
                    {
                        yield break;
                    }
                }

                var value = source.Substring(index, matchLength);

                var enterInCurrentValue = _regexIncludeReturn.Matches(value);

                if (!matchedDefinition.IsIgnored)
                {
                    yield return new Token(matchedDefinition.TokenType, matchedDefinition.TokenTypeString, value, new TokenPosition(index, line, column, value.Length));
                }

                if (enterInCurrentValue.Count > 0)
                {
                    line += enterInCurrentValue.Count;
                    column = value.Length - enterInCurrentValue[enterInCurrentValue.Count - 1].Length - enterInCurrentValue[enterInCurrentValue.Count - 1].Index + 1;
                }

                var whitespaceAfterMatch = _regexWhitespaceAndReturn.Match(source, index + matchLength);
                var newLinesAfterMatch = whitespaceAfterMatch.Groups["NewLine"];

                if (whitespaceAfterMatch.Success && whitespaceAfterMatch.Length > 0)
                {
                    if (!ignoreWhitespace)
                    {
                        /* TODO write new lines in Token (replace -10000 with value)
                         * var enterInWhitespaceAM = _includeReturn.Matches(source, index + matchLength);
                         * int enterLengthInWhitespaceAM = GetResultLengthFromRegexCollention(enterInWhitespaceAM);
                         */
                        if (newLinesAfterMatch.Success)
                        {
                            yield return new Token(TokenType.ENTER, null, whitespaceAfterMatch.Value,
                                new TokenPosition(whitespaceAfterMatch.Index, line, column, -10000));
                        }
                        else
                        {
                            yield return new Token(TokenType.WHITESPACE, null, whitespaceAfterMatch.Value,
                                new TokenPosition(whitespaceAfterMatch.Index, line, column, -10000));
                        }
                    }

                    matchLength += whitespaceAfterMatch.Length;

                    if (newLinesAfterMatch.Success)
                    {
                        line += newLinesAfterMatch.Captures.Count;
                        column = whitespaceAfterMatch.Length - (whitespaceAfterMatch.Value.LastIndexOf(newLinesAfterMatch.Value) + 1);
                    }
                    else if (enterInCurrentValue.Count == 0)
                    {
                        column += matchLength;
                    }
                }
                else if (enterInCurrentValue.Count == 0)
                {
                    column += matchLength;
                }

                index += matchLength;
            }
        }
    }
}