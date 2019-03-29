using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RC_SpeechToText.Models
{
    public class CommonSubsequence
{
        public List<string> longestCommonSub { get; set; }
        public List<int> oldTransPositions { get; set; }
        public List<int> newTransPosition { get; set; }
        public List<string> newTranscriptionTerms { get; set; }
    }
}
