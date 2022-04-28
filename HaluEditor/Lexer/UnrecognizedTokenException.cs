using System;

namespace ngprojects.HaluEditor.Lexer
{
    public class UnrecognizedTokenException : Exception
    {
        public UnrecognizedTokenException(char symbol, TokenPosition position)
        {
            Position = position;
            Symbol = symbol;
        }

        public UnrecognizedTokenException(char symbol, TokenPosition position, string message) : base(message)
        {
            Position = position;
            Symbol = symbol;
        }

        public UnrecognizedTokenException(char symbol, TokenPosition position, string message, Exception innerException) : base(message, innerException)
        {
            Position = position;
            Symbol = symbol;
        }

        /// <summary>
        /// The position in the document/stream where the error occurred.
        /// </summary>
        public TokenPosition Position { get; }

        /// <summary>
        /// The symbol that wasn't recognized.
        /// </summary>
        public char Symbol { get; }
    }
}