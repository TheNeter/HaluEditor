using System;
using System.Text.RegularExpressions;

namespace ngprojects.HaluEditor.Lexer
{
    /// <summary>
    /// Defines a <see cref="Token" /> type.
    /// </summary>
    public class TokenDefinition
    {
        private string _tokenTypeString;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given pattern and type.
        /// </summary>
        /// <param name="regex"> The pattern used to find this token. </param>
        /// <param name="type">  The type of this token. </param>
        public TokenDefinition(string regex, TokenType type)
            : this(new Regex(regex), type, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given pattern and type.
        /// </summary>
        /// <param name="regex"> The pattern used to find this token. </param>
        /// <param name="type"> 
        /// The name of the type of this token. The <see cref="TokenType" /> will be <see cref="TokenType.NULL" />.
        /// </param>
        public TokenDefinition(string regex, string type)
            : this(new Regex(regex), TokenType.NULL, type, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given regex and type.
        /// </summary>
        /// <param name="regex"> The regex used to find this token. </param>
        /// <param name="type">  The type of this token. </param>
        public TokenDefinition(Regex regex, TokenType type)
            : this(regex, type, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given regex and type.
        /// </summary>
        /// <param name="regex"> The regex used to find this token. </param>
        /// <param name="type"> 
        /// The name of the type of this token. The <see cref="TokenType" /> will be <see cref="TokenType.NULL" />.
        /// </param>
        public TokenDefinition(Regex regex, string type)
            : this(regex, TokenType.NULL, type, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given pattern and type, and determines whether it should be ignored or not.
        /// </summary>
        /// <param name="regex">     The pattern used to find this token. </param>
        /// <param name="type">      The type of this token. </param>
        /// <param name="isIgnored"> </param>
        public TokenDefinition(string regex, TokenType type, bool isIgnored)
            : this(new Regex(regex), type, null, isIgnored)
        {
        }

        /// <summary>
        /// /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given pattern and type, and determines whether it should be ignored or not.
        /// </summary>
        /// <param name="regex">     The pattern used to find this token. </param>
        /// <param name="type">     
        /// The name of the type of this token. The <see cref="TokenType" /> will be <see cref="TokenType.NULL" />.
        /// </param>
        /// <param name="isIgnored"> </param>
        public TokenDefinition(string regex, string type, bool isIgnored)
            : this(new Regex(regex), TokenType.NULL, type, isIgnored)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition" />. A token using the
        /// given regex and type, and determines whether it should be ignored or not.
        /// </summary>
        /// <param name="regex">      The regex used to find this token. </param>
        /// <param name="type">       The name of the type of this token. </param>
        /// <param name="typestring">
        /// The name of the type of this token. When used the param <see cref="TokenType" /> should
        /// be <see cref="TokenType.NULL" />
        /// </param>
        /// <param name="isIgnored">  </param>
        public TokenDefinition(Regex regex, TokenType type, string typestring, bool isIgnored)
        {
            Regex = regex ?? throw new ArgumentNullException("regex", "The regex for a TokenDefinition can't be null.");
            if (type.Equals(TokenType.NULL) && string.IsNullOrEmpty(typestring))
            {
                throw new ArgumentNullException("typestring", "The typestring for a TokenDefinition can't be null, when the type is null.");
            }
            if (!type.Equals(TokenType.NULL) && !string.IsNullOrEmpty(typestring))
            {
                throw new ArgumentNullException("typestring", "The typestring for a TokenDefinition can't be set, when the type is set.");
            }
            TokenType = type;
            TokenTypeString = typestring;
            IsIgnored = isIgnored;
        }

        /// <summary>
        /// Determines if this token type is returned when found while tokenizing.
        /// </summary>
        public bool IsIgnored { get; }

        /// <summary>
        /// The regex used to find this token.
        /// </summary>
        public Regex Regex { get; }

        /// <summary>
        /// The type of this token.
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// The type of this token as string
        /// </summary>
        public string TokenTypeString
        {
            get
            {
                if (_tokenTypeString != null)
                {
                    return _tokenTypeString;
                }
                return TokenType.ToString();
            }
            private set
            {
                _tokenTypeString = value;
            }
        }
    }
}