using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IRE {

    public sealed partial class RegexBuilder {

        #region Member Variables and Constructors

        /// <summary>
        /// The <c>StringBuilder</c> that represents the current regular expression.
        /// </summary>
        private StringBuilder regexString = new StringBuilder();
        /// <summary>
        /// Specifies whether this regex supports itself and an empty string, or just itself.
        /// </summary>
        private bool allowEmptyString = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexBuilder"/> class.
        /// </summary>
        public RegexBuilder() {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexBuilder"/> class.
        /// </summary>
        /// <param name="allowEmptyString">if set to <c>true</c> [allow empty string].</param>
        public RegexBuilder(bool allowEmptyString) {
            if (allowEmptyString) {
                this.allowEmptyString = allowEmptyString;
                regexString.Append("(^$)|(");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the generated regular expression will match empty strings in additional to it's specified pattern.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows empty string]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsEmptyString {
            get {
                return this.allowEmptyString;
            }
        }

        #endregion

        #region Metacharacters

        /// <summary>
        /// Inserts the caret symbol which represents the start of a string
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder BeginString() {
            regexString.Append('^');
            
            return this;
        }

        /// <summary>
        /// Inserts the dollar-sign symbol which reprsents the end of a string
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder EndString() {
            regexString.Append('$');

            return this;
        }

        /// <summary>
        /// Appends text into the builder. Regex encodes <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The text to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AppendText(string input) {
            return AppendText(input, true);
        }

        /// <summary>
        /// Appends text into the builder
        /// </summary>
        /// <param name="input">The text to append</param>
        /// <param name="encode"><c>True</c>, to encode <paramref name="input"/>; <c>False</c> to append text as-is</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AppendText(string input, bool encode) {
            if (string.IsNullOrEmpty(input)) {
                return this;
            }

            regexString.Append(encode ? EscapeCharacters(input) : input);

            return this;
        }

        /// <summary>
        /// Appends text into the builder. Regex encodes <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The character to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AppendText(char input) {
            return AppendText(input, true);
        }

        /// <summary>
        /// Appends text into the builder
        /// </summary>
        /// <param name="input">The character to append</param>
        /// <param name="encode"><c>True</c>, to encode <paramref name="input"/>; <c>False</c> to append the character as-is</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AppendText(char input, bool encode) {
            return AppendText(input.ToString(), encode);
        }

        /// <summary>
        /// Inserts the pipe symbol which represents a logical OR
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Or() {
            this.regexString.Append('|');

            return this;
        }

        /// <summary>
        /// Appends a list of subclauses into the current builder, with each clause appended with a logical OR
        /// </summary>
        /// <param name="clauses">The subclauses to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Alternation(params Action<RegexBuilder>[] clauses) {
            if (clauses == null || clauses.Length == 0) {
                return this;
            }

            bool isFirst = true;
            foreach (var option in clauses) {
                if (isFirst) {
                    isFirst = false;
                }
                else {
                    regexString.Append('|');
                }

                option.Invoke(this);
            }

            return this;
        }

        /// <summary>
        /// Appends a list of subclauses into the current builder, with each clause appended with a logical OR
        /// </summary>
        /// <param name="isCaptureGroup">Indicates if the array of clauses should be wrapped in a capture group</param>
        /// <param name="makeEachAlternateALogicalGrouping">Indicates if each clause within the array should be its own capture group</param>
        /// <param name="clauses">The subclauses to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Alternation(bool isCaptureGroup = true, bool makeEachAlternateALogicalGrouping = false, params Action<RegexBuilder>[] clauses) {
            if (clauses == null || clauses.Length == 0) {
                return this;
            }

            if (isCaptureGroup) {
                regexString.Append('(');
            }

            bool isFirst = true;
            foreach (var option in clauses) {
                if (isFirst) {
                    isFirst = false;
                }
                else {
                    regexString.Append('|');
                }

                if (makeEachAlternateALogicalGrouping) {
                    CaptureGroup(option);
                }
                else {
                    option.Invoke(this);
                }
            }

            if (isCaptureGroup) {
                regexString.Append(')');
            }

            return this;
        }

        /// <summary>
        /// Appends an explicit quantifier to the previous expression
        /// </summary>
        /// <param name="quantity">The explicit quantity to require for the previous expression</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when <paramref name="quantity"/> is less than one.</exception>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Quantifier(int quantity) {
            if (quantity < 1) {
                throw new ArgumentOutOfRangeException("quantity", "quantity cannot be less than one.");
            }

            regexString.Append('{');
            regexString.Append(quantity);
            regexString.Append('}');

            return this;
        }

        /// <summary>
        /// Appends an explicit quantifier range to the previous expression. Does not use lazy evaluation.
        /// </summary>
        /// <param name="minimumOccurrences">The minimum allowed number of occurrences of the previous expression</param>
        /// <param name="maximumOccurrences">The maximum allowed number of occurrences of the previous expression</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder QuantifierRange(int minimumOccurrences, int maximumOccurrences) {
            return QuantifierRange(minimumOccurrences, maximumOccurrences, false);
        }

        /// <summary>
        /// Appends an explicit quantifier range to the previous expression
        /// </summary>
        /// <param name="minimumOccurrences">The minimum allowed number of occurrences of the previous expression</param>
        /// <param name="maximumOccurrences">The maximum allowed number of occurrences of the previous expression</param>
        /// <param name="lazyEvaluation">Indicates if the Regex interpreter should use lazy evaluation</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder QuantifierRange(int minimumOccurrences, int maximumOccurrences, bool lazyEvaluation) {
            if (minimumOccurrences < 0) {
                throw new ArgumentOutOfRangeException("minimumOccurrences");
            }
            else if (maximumOccurrences < 0) {
                throw new ArgumentOutOfRangeException("maximumOccurrences");
            }
            else if (maximumOccurrences <= minimumOccurrences) {
                throw new ArgumentOutOfRangeException("maximumOccurrences");
            }

            regexString.Append('{');
            regexString.Append(minimumOccurrences);
            regexString.Append(',');
            regexString.Append(maximumOccurrences);
            regexString.Append('}');

            if (lazyEvaluation) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Appends an explicit minimum quantifier to the previous expression
        /// </summary>
        /// <param name="minimumOccurrences">The minimum allowed number of occurrences of the previous expression</param>
        /// <param name="lazyEvaluation">Indicates if the Regex interpreter should use lazy evaluation</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder QuantifierMinimum(int minimumOccurrences, bool lazyEvaluation = false) {
            if (minimumOccurrences < 0) {
                throw new ArgumentOutOfRangeException("minimumOccurrences");
            }

            regexString.Append('{');
            regexString.Append(minimumOccurrences);
            regexString.Append(",}");

            if (lazyEvaluation) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Appends an explicit set of characters to match
        /// </summary>
        /// <param name="characters">The character set to append</param>
        /// <param name="encode"><c>True</c>, to encode <paramref name="characters"/>; <c>False</c> to append the character set as-is</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder MatchCharacterSet(string characters, bool encode = true) {
            if (string.IsNullOrEmpty(characters)) {
                return this;
            }

            regexString.Append('[');
            regexString.Append(encode ? EscapeCharacters(characters) : characters);
            regexString.Append(']');

            return this;
        }

        /// <summary>
        /// Appends an explicit set of characters to match
        /// </summary>
        /// <param name="characters">The character set to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder MatchCharacterSet(params char[] characters) {
            if (characters == null || characters.Length == 0) {
                return this;
            }

            return MatchCharacterSet(new string(characters));
        }

        /// <summary>
        /// Appends an explicit set of characters that should not match
        /// </summary>
        /// <param name="characters">The character set to append</param>
        /// <param name="encode"><c>True</c>, to encode <paramref name="characters"/>; <c>False</c> to append the character set as-is</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder NonMatchCharacterSet(string characters, bool encode = true) {
            if (string.IsNullOrEmpty(characters)) {
                return this;
            }

            regexString.Append('[');
            regexString.Append('^');
            regexString.Append(encode ? EscapeCharacters(characters) : characters);
            regexString.Append(']');

            return this;
        }

        /// <summary>
        /// Appends an explicit set of characters that should not match
        /// </summary>
        /// <param name="characters">The character set to append</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder NonMatchCharacterSet(params char[] characters) {
            if (characters == null || characters.Length == 0) {
                return this;
            }

            return NonMatchCharacterSet(new string(characters));
        }

        /// <summary>
        /// Appends a capture group that contains <paramref name="clause"/>
        /// </summary>
        /// <param name="clause">The clause to append</param>
        /// <param name="nonCapturing">If <c>True</c>, indicates that the group should not capture its contents; otherwise, <c>False</c></param>
        /// <param name="isOptionalGroup">If <c>True</c>, group is optional; otherwise, <c>False</c></param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder CaptureGroup(Action<RegexBuilder> clause, bool nonCapturing = false, bool isOptionalGroup = false) {
            if (clause == null) {
                return this;
            }

            regexString.Append('(');
            if (nonCapturing) {
                regexString.Append("?:");
            }

            clause.Invoke(this);
            regexString.Append(')');

            if (isOptionalGroup) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Allows zero or more of the previous expression. Does not use lazy evaluation.
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder OptionallyAnyQuantity() {
            return OptionallyAnyQuantity(false);
        }

        /// <summary>
        /// Allows zero or more of the previous expression.
        /// </summary>
        /// <param name="lazyEvaluation">if <c>true</c> forces this expression to use lazy evaluation.</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder OptionallyAnyQuantity(bool lazyEvaluation) {
            regexString.Append('*');

            if (lazyEvaluation) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Allows one or more of the previous expression. Does not use lazy evaluation.
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AtLeastOne() {
            return AtLeastOne(false);
        }

        /// <summary>
        /// Allows one or more of the previous expression.
        /// </summary>
        /// <param name="lazyEvaluation">if <c>true</c> forces this expression to use lazy evaluation.</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AtLeastOne(bool lazyEvaluation) {
            regexString.Append('+');

            if (lazyEvaluation) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Allows zero or one of the previous expression. Does not use lazy evaluation.
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Optional() {
            return Optional(false);
        }

        /// <summary>
        /// Allows zero or one of the previous expression.
        /// </summary>
        /// <param name="lazyEvaluation">if <c>true</c> forces this expression to use lazy evaluation.</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder Optional(bool lazyEvaluation) {
            regexString.Append('?');

            if (lazyEvaluation) {
                regexString.Append('?');
            }

            return this;
        }

        /// <summary>
        /// Inserts an inline comment into the expression
        /// </summary>
        /// <param name="comment">The comment to insert into the expression.</param>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AddComment(string comment) {
            if (string.IsNullOrEmpty(comment)) {
                return this;
            }

            regexString.Append("(?#");
            regexString.Append(comment);
            regexString.Append(')');

            return this;
        }

        #endregion

        #region Look Around

        public RegexBuilder PositiveLookAhead(Action<RegexBuilder> action) {
            if (action == null) {
                return this;
            }

            regexString.Append("(?=");
            action.Invoke(this);
            regexString.Append(')');

            return this;
        }

        public RegexBuilder NegativeLookAhead(Action<RegexBuilder> action) {
            if (action == null) {
                return this;
            }

            regexString.Append("(?!");
            action.Invoke(this);
            regexString.Append(')');

            return this;
        }

        public RegexBuilder PositiveLookBehind(Action<RegexBuilder> action) {
            if (action == null) {
                return this;
            }

            regexString.Append("(?<=");
            action.Invoke(this);
            regexString.Append(')');

            return this;
        }

        public RegexBuilder NegativeLookBehind(Action<RegexBuilder> action) {
            if (action == null) {
                return this;
            }

            regexString.Append("(?<!");
            action.Invoke(this);
            regexString.Append(')');

            return this;
        }

        #endregion

        #region CharacterClasses

        public RegexBuilder MatchAnyCharacterInNamedClass(string className) {
            regexString.Append("\\p");
            regexString.Append('{');
            regexString.Append(className);
            regexString.Append('}');

            return this;
        }

        public RegexBuilder MatchTextNotInNamedGroupOrBlockRange(string groupName) {
            regexString.Append("\\P");
            regexString.Append('{');
            regexString.Append(groupName);
            regexString.Append('}');

            return this;
        }

        /// <summary>
        /// Appends \w, which matches any word character
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyWordCharacter() {
            regexString.Append(@"\w");

            return this;
        }

        /// <summary>
        /// Appends \W, which matches any non-word character
        /// </summary>
        /// <returns></returns>
        public RegexBuilder AnyNonWordCharacter() {
            regexString.Append(@"\W");

            return this;
        }

        /// <summary>
        /// Appends \s, which matches any whitespace character
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyWhiteSpaceCharacter() {
            regexString.Append(@"\s");

            return this;
        }

        /// <summary>
        /// Appends \S, which matches any nonwhitespace character
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyNonWhiteSpaceCharacter() {
            regexString.Append(@"\S");

            return this;
        }

        /// <summary>
        /// Appends \d, which matches any decimal digit
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyDecimalDigit() {
            regexString.Append(@"\d");

            return this;
        }

        /// <summary>
        /// Appends \D, which matches any nondigit
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyNonDigit() {
            regexString.Append(@"\D");

            return this;
        }

        /// <summary>
        /// Appends \b, which matches any word boundary
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyWordBoundary() {
            regexString.Append(@"\b");

            return this;
        }

        /// <summary>
        /// Appends \B, which matches any nonword boundary
        /// </summary>
        /// <returns></returns>
        public RegexBuilder AnyNonWordBoundary() {
            regexString.Append(@"\B");

            return this;
        }

        /// <summary>
        /// Appends ., which matches any character
        /// </summary>
        /// <returns>The current <c>RegexBuilder</c> instance</returns>
        public RegexBuilder AnyCharacter() {
            regexString.Append('.');

            return this;
        }

        #endregion

        #region Handling Methods

        /// <summary>
        /// Determines if the current builder represents a valid Regex.
        /// </summary>
        /// <returns><c>true</c> if the Regex is valid; otherwise, <c>false</c>.</returns>
        public bool IsValid() {
            try {
                new Regex(regexString.ToString() + (allowEmptyString ? ")" : string.Empty));
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the current builder represents a valid Regex.
        /// </summary>
        /// <param name="options">The options to use when creating the Regex.</param>
        /// <returns><c>true</c> if the Regex is valid; otherwise, <c>false</c>.</returns>
        public bool IsValid(RegexOptions options) {
            try {
                new Regex(regexString.ToString() + (allowEmptyString ? ")" : string.Empty), options);
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates a Regex from the builder.
        /// </summary>
        /// <returns>A Regex that is equivalent to this builder.</returns>
        public Regex CreateRegex() {
            return new Regex(regexString.ToString() + (allowEmptyString ? ")" : string.Empty));
        }

        /// <summary>
        /// Generates a Regex from the builder.
        /// </summary>
        /// <param name="options">The options to use when creating the Regex.</param>
        /// <returns>A Regex that is equivalent to this builder.</returns>
        public Regex CreateRegex(RegexOptions options) {
            return new Regex(regexString.ToString() + (allowEmptyString ? ")" : string.Empty), options);
        }

        /// <summary>
        /// Removes all constructs from this RegexBuilding instance.
        /// </summary>
        public void Clear() {
            regexString.Clear();
        }

        #endregion

    }

}
