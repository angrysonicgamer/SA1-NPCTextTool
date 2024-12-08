namespace SA1_NPCTextTool
{
    public static class NPCText
    {
        public static void Extract(string binFile, AppConfig config)
        {
            string jsonFile = $"{Path.GetFileNameWithoutExtension(binFile)}.json";
            JsonFile.Create(jsonFile, BinaryFile.Read(binFile, config));
            DisplayMessage.Config(config);
            DisplayMessage.TextExtracted(jsonFile);
        }

        public static void Import(string jsonFile, AppConfig config)
        {
            string binFile = $"{Path.GetFileNameWithoutExtension(jsonFile)}.bin";
            BinaryFile.Write(jsonFile, config);
            DisplayMessage.Config(config);
            DisplayMessage.FileCreated(binFile);
        }
    }
}
