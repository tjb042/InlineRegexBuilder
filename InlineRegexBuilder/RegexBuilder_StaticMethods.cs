using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRE {

    public partial class RegexBuilder {

        /// <summary>
        /// The character used to escape special characters.
        /// </summary>
        private const char escapeCharacter = '\\';

        /// <summary>
        /// Special characters that have to be espaced in a literal.
        /// </summary>
        private static char[] specialCharacters = new char[] { '^', '$', '.', '|', '{', '}', '[', ']', '(', ')', '*', '+', '?', '\\' };
        
        /// <summary>
        /// Escapes characters using the Regex escaping character
        /// </summary>
        /// <param name="input">The text to encode</param>
        /// <returns>A Regex encoded implementation of <paramref name="input"/></returns>
        private static string EscapeCharacters(string input) {
            if (string.IsNullOrEmpty(input)) {
                return input;
            }

            // assuming there's nothing to encode, the string will be the same length
            // if there are, the size will double, which should be able to handle 1 - N
            // (where N is the length of the string) replacements without having to do
            // another memory allocation
            StringBuilder builder = new StringBuilder(input.Length);
            char c;
            
            for (int i = 0; i < input.Length; i++) {
                c = input[i];
                if (specialCharacters.Contains(c)) {
                    builder.Append(escapeCharacter);
                }

                builder.Append(c);
            }

            return builder.ToString();
        }

    }

}
