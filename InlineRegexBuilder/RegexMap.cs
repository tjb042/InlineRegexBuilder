using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IRE {

    public class RegexMap<TKey> : Dictionary<TKey, Regex> {

        public RegexMap() : base() { }

        public RegexMap(int capacity) : base(capacity) { }

        public void Add(TKey key, RegexBuilder builder) {
            this.Add(key, builder.CreateRegex());
        }

        public void Add(TKey key, RegexBuilder builder, RegexOptions options) {
            this.Add(key, builder.CreateRegex(options));
        }

    }

}
