namespace ngprojects.HaluEditor.Lexer
{
    public class Token
    {
        internal Token(TokenType type, string tokenText, string value, TokenPosition position)
        {
            TokenType = type;
            TokenText = tokenText;
            Value = value;
            Position = position;
        }

        /// <summary>
        /// The location within the stream that this token was found.
        /// </summary>
        public TokenPosition Position { get; }

        public string TokenText { get; }
        public TokenType TokenType { get; }
        public string Value { get; }

        public override string ToString()
        {
            return $"Token: Type {TokenType} Text {TokenText} | Value {Value} | Position {Position}";
        }
    }
}