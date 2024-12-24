using System.Text.Json.Serialization;

namespace SA1_NPCTextTool
{
    public enum Encodings
    {
        Windows1251 = 1251,
        Windows1252 = 1252,
        ShiftJIS = 932
    }
        
    public class NPCTextEntry
    {
        private static int npcCount = 0;


        [JsonPropertyName("NPC ID")]
        public int NPCID { get; set; }
        public List<List<string>> Strings { get; set; }

        [JsonConstructor]
        public NPCTextEntry() { }


        public void Read(BinaryReader reader, AppConfig config, uint baseAddress)
        {
            uint flagsPtr = reader.ReadUInt32();
            uint linePtr = reader.ReadUInt32();
            long returnToThisPosition = reader.BaseStream.Position;

            int groupCount = GetGroupCount(reader, flagsPtr, baseAddress);
            reader.SetPosition(linePtr - baseAddress);
            npcCount++;
            NPCID = npcCount;
            Strings = GetGroupedStrings(reader, config, groupCount, baseAddress);
            reader.SetPosition(returnToThisPosition);
        }


        private static int GetGroupCount(BinaryReader reader, uint flagsPointer, uint baseAddress)
        {
            int groupCount = 1;
            short newGroupFlag = -2;
            short endFlag = -1;

            if (flagsPointer != 0)
            {
                reader.SetPosition(flagsPointer - baseAddress);

                while (true)
                {
                    short flagtype = reader.ReadInt16();
                    if (flagtype == endFlag) break;
                    if (flagtype == newGroupFlag)
                        groupCount++;
                }
            }

            return groupCount;
        }

        private static List<List<string>> GetGroupedStrings(BinaryReader reader, AppConfig config, int groupCount, uint baseAddress)
        {
            int currentGroup = 1;
            var groupedStrings = new List<List<string>>() { new List<string>() };

            while (true)
            {
                int textPtr = reader.ReadInt32();

                if (textPtr == 0)
                {
                    currentGroup++;
                    if (currentGroup > groupCount) break;
                    groupedStrings.Add(new List<string>());
                    continue;
                }

                string text = reader.ReadAt(textPtr - baseAddress, x => x.ReadCString(config.Encoding));
                groupedStrings.Last().Add(text.Replace("\a", "")); // assuming all strings would use "block" centering (text starts with \a), you can add \t to use "each line" centering
            }

            return groupedStrings;
        }
    }

    public class NPCTextFile
    {
        public string Name { get; set; }

        [JsonPropertyName("NPC Text")]
        public List<NPCTextEntry> NPCText { get; set; }

        [JsonConstructor]
        public NPCTextFile() { }

        public NPCTextFile(string name, List<NPCTextEntry> npcText)
        {
            Name = name;
            NPCText = npcText;
        }
    }
}
