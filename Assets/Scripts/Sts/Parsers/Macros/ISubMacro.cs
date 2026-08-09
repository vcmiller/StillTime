using System.Collections.Generic;

namespace StillTime.Sts.Parsers.Macros {
    public interface ISubMacro {
        public IEnumerable<string> Expand(LineTokens callTokens);
    }
}
