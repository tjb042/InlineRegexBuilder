using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRE {

    public partial class RegexBuilder {

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            return Equals(obj as RegexBuilder);
        }

        /// <summary>
        /// Determines whether the specified <see cref="RegularExpressionBuilder.RegexBuilder"/> is equal to this instance.
        /// </summary>
        /// <param name="builder">The builder to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="RegularExpressionBuilder.RegexBuilder"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RegexBuilder builder) {
            if (builder == null) {
                return false;
            }

            return builder.regexString.Equals(this.regexString);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return this.regexString.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            if (allowEmptyString) {
                return regexString.ToString() + ')';
            }

            return regexString.ToString();
        }

    }

}
