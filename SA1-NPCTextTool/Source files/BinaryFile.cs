﻿namespace SA1_NPCTextTool
{
    public static class BinaryFile
    {
        private static uint baseAddress;

        private static readonly Dictionary<string, uint> baseAddresses = new()
        {
            { "ss", 0xCB46000 },
            { "mr", 0xCB4A000 },
            { "past", 0xCB4A000 },
        };

        private static void SetBaseAddress(string fileName)
        {
            string location = fileName.Substring(0, fileName.IndexOf('_')).ToLower();
            baseAddress = baseAddresses[location];
        }
                

        public static JsonContents Read(string binFile, AppConfig config)
        {
            var source = File.ReadAllBytes(binFile);
            string name = Path.GetFileNameWithoutExtension(binFile);
            SetBaseAddress(name);
            var reader = new BinaryReader(new MemoryStream(source));

            // Reading pointers and stuff

            reader.BaseStream.Position = 4;
            uint flagsAndLinePtrs = reader.ReadUInt32() - baseAddress;
            reader.BaseStream.Position = flagsAndLinePtrs;

            var linePointers = new List<uint>();
            var flagPointers = new List<uint>();

            while (true)
            {
                uint flagsPtr = reader.ReadUInt32();
                uint linePtr = reader.ReadUInt32();
                if (flagsPtr > 0xCB50000) break; // pretty stupid condition LUL

                linePointers.Add(linePtr);
                flagPointers.Add(flagsPtr);
            }

            var groupCounts = new List<int>();

            foreach (var ptr in flagPointers)
            {
                int groupCount = 1;
                short newGroupFlag = -2;
                short endFlag = -1;

                if (ptr != 0)
                {
                    reader.BaseStream.Position = ptr - baseAddress;

                    while (true)
                    {
                        short flagtype = reader.ReadInt16();
                        if (flagtype == endFlag) break;
                        if (flagtype == newGroupFlag)
                            groupCount++;
                    }
                }

                groupCounts.Add(groupCount);
            }

            // Reading text

            var allNPCsText = new JsonContents(name, new List<NPCTextEntry>());
            var npcText = new List<NPCTextEntry>();

            for (int i = 0; i < linePointers.Count; i++)
            {
                reader.BaseStream.Position = linePointers[i] - baseAddress;
                int npcID = i + 1;
                int groupCount = groupCounts[i];
                int currentGroup = 1;
                var groupedStrings = new List<List<string>>() { new List<string>() };

                while (true)
                {
                    var pos = reader.BaseStream.Position;
                    int textptr = reader.ReadInt32();
                    if (textptr == 0)
                    {
                        currentGroup++;
                        if (currentGroup > groupCount) break;
                        groupedStrings.Add(new List<string>());
                        continue;
                    }

                    reader.BaseStream.Position = textptr - baseAddress;
                    string text = reader.ReadCString(config.Encoding).Replace("\a", "");
                    groupedStrings.Last().Add(text);
                    reader.BaseStream.Position = pos + 4;
                }

                var currentNPCText = new NPCTextEntry(npcID, groupedStrings);
                allNPCsText.NPCText.Add(currentNPCText);
            }

            return allNPCsText;
        }

        public static void Write(JsonContents jsonContents, AppConfig config)
        {
            string name = jsonContents.Name;
            string binFile = $"{name}.bin";
            string createdFilesDir = "New files";           

            if (!File.Exists(binFile))
            {
                DisplayMessage.FileNotFound(binFile);
                return;
            }

            SetBaseAddress(name);

            // Reading corresponding bin file and copying its beginning (flags and stuff) to a new byte array

            var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(binFile)));
            reader.BaseStream.Position = 4;
            uint flagsAndTextPointersOffset = reader.ReadUInt32() - baseAddress;
            reader.BaseStream.Position = flagsAndTextPointersOffset;

            while (true)
            {
                uint flagsPtr = reader.ReadUInt32();
                if (flagsPtr > 0xCB50000) break; // pretty stupid condition again LUL
                reader.BaseStream.Position += 4;
            }

            var firstTextOffset = reader.BaseStream.Position - 4;
            reader.BaseStream.Position = 0;
            var outputFile = new List<byte>();
            outputFile.AddRange(reader.ReadBytes((int)firstTextOffset));

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

            outputFile.AddRange(cStrings);
            uint textEnd = (uint)outputFile.Count;

            // Adding text pointers

            foreach (var ptr in linePointers)
            {
                outputFile.AddRange(BitConverter.GetBytes(ptr));
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

            // Creating a new bin file

            byte[] output = outputFile.ToArray();
            var writer = new BinaryWriter(new MemoryStream(output));
            writer.BaseStream.Position = flagsAndTextPointersOffset + 4;

            for (int i = 0; i < textPointers.Count; i++)
            {
                writer.Write(textPointers[i]);
                writer.BaseStream.Position += 4;
            }

            reader.Dispose();
            writer.Dispose();

            Directory.CreateDirectory(createdFilesDir);
            File.WriteAllBytes($"{createdFilesDir}\\{binFile}", output);
            DisplayMessage.Config(config);
            DisplayMessage.NPCFileCreated(binFile);
        }
    }
}
