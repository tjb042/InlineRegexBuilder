using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IRE {

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2229:ImplementSerializationConstructors"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [Serializable]
    public class RegexMap<TKey> : Dictionary<TKey, Regex> {

        public RegexMap() : base() { }

        public RegexMap(int capacity) : base(capacity) { }

        public void Add(TKey key, RegexBuilder builder) {
            if (key == null) {
                throw new ArgumentNullException("key");
            }
            if (builder == null) {
                throw new ArgumentNullException("builder");
            }

            this.Add(key, builder.CreateRegex());
        }

        public void Add(TKey key, RegexBuilder builder, RegexOptions options) {
            if (key == null) {
                throw new ArgumentNullException("key");
            }
            if (builder == null) {
                throw new ArgumentNullException("builder");
            }

            this.Add(key, builder.CreateRegex(options));
        }

    }

}
