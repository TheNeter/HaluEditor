namespace ngprojects.HaluEditor.Lexer
{
    public enum TokenType
    {
        /// <summary>
        /// System TokenType to set tokenType by string-value
        /// </summary>
        NULL,

        /// <summary>
        /// System TokenType when the <see cref="Lexer" /> returns whitespace <see cref="Token" />
        /// </summary>
        WHITESPACE,

        /// <summary>
        /// System TokenType when the <see cref="Lexer" /> returns Enter <see cref="Token" />
        /// </summary>
        ENTER,

        Identifier_Def,
        Identifier_Keyword,
        Operator,
        Literal_Number,
        Literal_String
    }
}