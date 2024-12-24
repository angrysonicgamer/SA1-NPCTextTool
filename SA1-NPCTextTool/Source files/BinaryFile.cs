﻿namespace SA1_NPCTextTool
{
    public static class BinaryFile
    {
        private static uint baseAddress;
        private static int baseCount;

        private static readonly Dictionary<string, uint> baseAddresses = new()
        {
            { "ss", 0xCB46000 },
            { "mr", 0xCB4A000 },
            { "past", 0xCB4A000 },
        };

        private static readonly Dictionary<string, int> baseCounts = new()
        {
            { "ss", 36 },
            { "mr", 5 },
            { "past", 20 },
        };

        private static void SetBaseValues(string fileName)
        {
            string location = fileName.Substring(0, fileName.IndexOf('_')).ToLower();
            baseAddress = baseAddresses[location];
            baseCount = baseCounts[location];
        }
                

        public static NPCTextFile Read(string binFile, AppConfig config)
        {
            DisplayMessage.ReadingFile(binFile);
            var source = File.ReadAllBytes(binFile);
            string name = Path.GetFileNameWithoutExtension(binFile);
            SetBaseValues(name);

            var reader = new BinaryReader(new MemoryStream(source));
            int npcCount = reader.ReadInt32() + baseCount;
            uint flagsAndTextPointersOffset = reader.ReadUInt32() - baseAddress;
            reader.SetPosition(flagsAndTextPointersOffset);
            var allNPCsText = new NPCTextFile(name, new List<NPCTextEntry>());

            for (int i = 0; i < npcCount; i++)
            {
                var entry = new NPCTextEntry();
                entry.Read(reader, config, baseAddress);
                allNPCsText.NPCText.Add(entry);
            }

            return allNPCsText;
        }

        public static void Write(NPCTextFile jsonContents, AppConfig config)
        {
            string name = jsonContents.Name;
            string binFile = $"{name}.bin";
            string createdFilesDir = "New files";
            Directory.CreateDirectory(createdFilesDir);

            if (!File.Exists(binFile))
            {
                DisplayMessage.FileNotFound(binFile);
                return;
            }

            SetBaseValues(name);

            // Reading corresponding bin file and copying its beginning (flags and stuff) to a new byte array

            var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(binFile)));
            reader.SetPosition(4);
            uint flagsAndTextPointersOffset = reader.ReadUInt32() - baseAddress;
            var firstTextOffset = flagsAndTextPointersOffset + jsonContents.NPCText.Count * 2 * sizeof(int);
            reader.SetPosition(0);

            var writer = new BinaryWriter(File.Create($"{createdFilesDir}\\{binFile}"));
            writer.Write(reader.ReadBytes((int)firstTextOffset)); // copying source file contents up to first text strings

            // Adding strings, calculating text pointers

            var cStrings = new List<byte>();
            var linePointers = new List<uint>();
            var textPointers = new List<uint>();

            foreach (var npcID in jsonContents.NPCText)
            {
                foreach (var group in npcID.Strings)
                {
                    foreach (var str in group)
                    {
                        string text = str.StartsWith('\t') ? str : "\a" + str;
                        var textBytes = new List<byte>();
                        textBytes.AddRange(config.Encoding.GetBytes(text));
                        textBytes.Add(0);
                        int extra = textBytes.Count % 4;
                        textBytes.AddRange(new byte[(4 - extra) % 4]);
                        cStrings.AddRange(textBytes);

                        linePointers.Add((uint)firstTextOffset + baseAddress);
                        firstTextOffset += textBytes.Count;
                    }

                    linePointers.Add(0);
                }
            }

            writer.Write(cStrings.ToArray());
            uint textEnd = (uint)writer.BaseStream.Length;

            // Adding text pointers

            foreach (var ptr in linePointers)
            {
                writer.Write(ptr);
            }

            uint totalCount = 0;

            foreach (var npcID in jsonContents.NPCText)
            {
                textPointers.Add(textEnd + 4 * totalCount + baseAddress);

                foreach (var group in npcID.Strings)
                {
                    totalCount += (uint)group.Count + 1;
                }
            }

            writer.BaseStream.Position = flagsAndTextPointersOffset + 4;

            for (int i = 0; i < textPointers.Count; i++)
            {
                writer.Write(textPointers[i]);
                writer.BaseStream.Position += 4;
            }

            reader.Dispose();
            writer.Dispose();

            DisplayMessage.Config(config);
            DisplayMessage.NPCFileCreated(binFile);
        }
    }
}
