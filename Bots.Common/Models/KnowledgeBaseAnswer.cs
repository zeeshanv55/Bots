namespace Bots.Common.Models
{
    using System.Collections.Generic;

    public class KnowledgeBaseAnswer
    {
        public List<string> Questions { get; set; }

        public string Answer { get; set; }

        public float Score { get; set; }

        public int Id { get; set; }
    }
}
