using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IRE {

    public partial class RegexBuilder {

        /// <summary>
        /// Performs an explicit conversion from <see cref="RegularExpressionBuilder.RegexBuilder"/> to <see cref="System.Text.RegularExpressions.Regex"/>.
        /// </summary>
        /// <param name="builder">The builder to convert into a Regex.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Regex(RegexBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException("builder");
            }

            return builder.CreateRegex();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Text.RegularExpressions.Regex"/> to <see cref="RegularExpressionBuilder.RegexBuilder"/>.
        /// </summary>
        /// <param name="regex">The regex to convert into a RegexBuilder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator RegexBuilder(Regex regex) {
            if (regex == null) {
                throw new ArgumentNullException("regex");
            }

            RegexBuilder builder = new RegexBuilder(false);
            builder.AppendText(regex.ToString(), false);

            return builder;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="RegularExpressionBuilder.RegexBuilder"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="builder">The builder to convert into a Regex.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator string(RegexBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException("builder");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="RegularExpressionBuilder.RegexBuilder"/>.
        /// </summary>
        /// <param name="regex">The string to convert into a RegexBuilder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator RegexBuilder(string regex) {
            RegexBuilder builder = new RegexBuilder();
            if (string.IsNullOrEmpty(regex)) {
                return builder;
            }

            builder.AppendText(regex, false);
            if (!builder.IsValid()) {
                throw new InvalidOperationException("The supplied input 'regex' was not a valid regular expression.");
            }

            return builder;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="builderA">The 'operator' builder.</param>
        /// <param name="builderB">The 'operand' builder.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static RegexBuilder operator +(RegexBuilder builderA, RegexBuilder builderB) {
            return Add(builderA, builderB);
        }

        /// <summary>
        /// Appends two builders together.
        /// </summary>
        /// <param name="builderA">The 'operator' builder.</param>
        /// <param name="builderB">The 'operand' builder.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static RegexBuilder Add(RegexBuilder builderA, RegexBuilder builderB) {
            // if A allows emptys it doesn't have its closing tag, so don't worry
            // if B allows emptys it doesn't have a closing tag and we want to ignore the first (^$)|( characters
            if (builderA == null) {
                throw new ArgumentNullException("builderA");
            }
            if (builderB == null) {
                throw new ArgumentNullException("builderB");
            }

            builderA.regexString.Append(builderB.regexString.ToString().Substring(builderB.AllowsEmptyString ? 6 : 0));

            return builderA;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "builderB")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "builderA")]
        public static RegexBuilder operator -(RegexBuilder builderA, RegexBuilder builderB) {
            throw new NotSupportedException();
        }

        public static bool operator ==(RegexBuilder builderA, RegexBuilder builderB) {
            if (builderA == null) {
                if (builderB == null) {
                    return true;
                }

                return false;
            }

            return builderA.Equals(builderB);
        }

        public static bool operator !=(RegexBuilder builderA, RegexBuilder builderB) {
            return !(builderA == builderB);
        }

    }

}
