namespace Discord.JonSnowBot.DialogueExtractor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var allTranscripts = File.ReadAllLines(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "transcripts.txt")).ToList();
            var allDialogue = allTranscripts.Where(s => s.Contains(":")).ToArray();
            var jonSnowDialogues = new List<JonSnowDialogue>();
            
            var dialogueCollection = string.Empty;
            var dialogueCollectionStartIndex = -1;
            for (var i = 0; i < allDialogue.Length; i++)
            {
                if (IsSpokenByJon(allDialogue[i]))
                {
                    if (string.IsNullOrWhiteSpace(dialogueCollection))
                    {
                        dialogueCollectionStartIndex = i;
                    }
                    
                    dialogueCollection += allDialogue[i].Substring(allDialogue[i].IndexOf(":") + 1).Trim() + " ";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dialogueCollection))
                    {
                        jonSnowDialogues.Add(new JonSnowDialogue
                        {
                            Dialogue = dialogueCollection.Trim(),
                            Index = dialogueCollectionStartIndex,
                            Trigger = allDialogue[dialogueCollectionStartIndex - 1].Substring(allDialogue[dialogueCollectionStartIndex - 1].IndexOf(":") + 1).Trim(),
                        });
                    }

                    dialogueCollection = string.Empty;
                    dialogueCollectionStartIndex = -1;
                }
            }

            var qnaStyleText = string.Empty;
            foreach (var dialogue in jonSnowDialogues)
            {
                qnaStyleText += $"Q: {dialogue.Trigger}\r\nA: {dialogue.Dialogue}\r\n\r\n";
            }

            File.WriteAllText(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "output.txt"), qnaStyleText);
        }

        static bool IsSpokenByJon(string dialogue)
        {
            return 
                dialogue.Contains(":") &&
                !dialogue.StartsWith("jonos bracken:", StringComparison.InvariantCultureIgnoreCase) &&
                (
                    dialogue.StartsWith("jon", StringComparison.InvariantCultureIgnoreCase) ||
                    dialogue.StartsWith("jon snow", StringComparison.InvariantCultureIgnoreCase)
                );
        }
    }

    public class JonSnowDialogue
    {
        public string Trigger { get; set; }

        public string Dialogue { get; set; }

        public int Index { get; set; }
    }
}
