using System.Text.Json.Serialization;

namespace SA1_NPCTextTool
{
    public enum Encodings
    {
        Windows1251 = 1251,
        Windows1252 = 1252,
        ShiftJIS = 932
    }

    public class JsonContents
    {
        public string Name { get; set; }

        [JsonPropertyName("NPC Text")]
        public List<NPCTextEntry> NPCText { get; set; }

        [JsonConstructor]
        public JsonContents() { }

        public JsonContents(string name, List<NPCTextEntry> npcText)
        {
            Name = name;
            NPCText = npcText;
        }
    }
    
    public class NPCTextEntry
    {
        [JsonPropertyName("NPC ID")]
        public int NPCID { get; set; }
        public List<List<string>> Strings { get; set; }

        [JsonConstructor]
        public NPCTextEntry() { }

        public NPCTextEntry(int id, List<List<string>> strings)
        {
            NPCID = id;
            Strings = strings;
        }
    }
}
