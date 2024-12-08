using System.Net.Http.Json;

namespace SA1_NPCTextTool
{
    public static class NPCText
    {
        public static void Extract(string binFile, AppConfig config)
        {
            string jsonFile = $"{Path.GetFileNameWithoutExtension(binFile)}.json";

            try
            {
                var jsonContents = BinaryFile.Read(binFile, config);
                JsonFile.Create(jsonFile, jsonContents, config);
            }
            catch (Exception ex)
            {
                DisplayMessage.WrongBinFile();
            }            
        }

        public static void Import(string jsonFile, AppConfig config)
        {
            string binFile = $"{Path.GetFileNameWithoutExtension(jsonFile)}.bin";
            JsonContents jsonContents = null;

            try
            {
                jsonContents = JsonFile.Read(jsonFile);
            }
            catch (Exception ex)
            {
                DisplayMessage.InvalidJson();
                return;
            }

            if (jsonContents == null || jsonContents.Name == null || jsonContents.NPCText == null)
            {
                DisplayMessage.WrongJsonFile();
                return;
            }

            BinaryFile.Write(jsonContents, config);
        }
    }
}
