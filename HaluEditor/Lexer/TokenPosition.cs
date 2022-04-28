namespace ngprojects.HaluEditor.Lexer
{
    /// <summary>
    /// Represents a position within a document/stream where a <see cref="Token" /> was found.
    /// </summary>
    public class TokenPosition
    {
        internal TokenPosition(int index, int line, int column, int length)
        {
            Index = index;
            Line = line;
            Column = column;
            Length = length;
        }

        /// <summary>
        /// The position on the line where the <see cref="Token" /> was found.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// The index in the document/stream where the <see cref="Token" /> was found.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The length without new lines
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The line number where the <see cref="Token" /> was found.
        /// </summary>
        public int Line { get; }

        public override string ToString()
        {
            return $"TokenPosition: Line: {Line} | Column: {Column} | Index: {Index} | Length: {Length}";
        }
    }
}