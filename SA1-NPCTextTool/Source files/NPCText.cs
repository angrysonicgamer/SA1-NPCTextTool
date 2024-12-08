namespace SA1_NPCTextTool
{
    public static class NPCText
    {
        public static void Extract(string binFile, AppConfig config)
        {
            string jsonFile = $"{Path.GetFileNameWithoutExtension(binFile)}.json";
            var jsonContents = BinaryFile.Read(binFile, config);
            JsonFile.Create(jsonFile, jsonContents, config);
        }

        public static void Import(string jsonFile, AppConfig config)
        {
            string binFile = $"{Path.GetFileNameWithoutExtension(jsonFile)}.bin";
            BinaryFile.Write(jsonFile, config);
        }
    }
}
