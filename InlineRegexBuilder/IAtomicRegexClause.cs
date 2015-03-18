using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRE {

    public interface IAtomicRegexClause {

        IAtomicRegexClause BeginString();

        IAtomicRegexClause EndString();

        IAtomicRegexClause AppendText(string input, bool encode);

        IAtomicRegexClause AppendText(char input, bool encode);

        IAtomicRegexClause Or();



    }

    public interface IModifyRegexClause {

        IAtomicRegexClause Quantifier(int quantity);

    }

    public interface IRegexClause : IAtomicRegexClause, IModifyRegexClause { }

}
