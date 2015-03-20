using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRE {

    public partial class RegexBuilder {

        /// <summary>
        /// Gets a value indicating whether the generated regular expression will match empty strings in additional to it's specified pattern.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows empty string]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsEmptyString {
            get;
            set;
        }

    }

}
