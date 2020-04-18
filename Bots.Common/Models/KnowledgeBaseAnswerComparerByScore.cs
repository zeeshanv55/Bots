namespace Bots.Common.Models
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class KnowledgeBaseAnswerComparerByScore : IComparer<KnowledgeBaseAnswer>
    {
        public int Compare([AllowNull] KnowledgeBaseAnswer x, [AllowNull] KnowledgeBaseAnswer y)
        {
            return 
                x.Score - y.Score < 0 
                ? 1 
                : x.Score == y.Score 
                    ? 0 
                    : -1;
        }
    }
}
