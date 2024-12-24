using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SA1_NPCTextTool
{
    public static class JsonFile
    {
        public static NPCTextFile Read(string jsonFile)
        {
            DisplayMessage.ReadingFile(jsonFile);
            var json = JsonNode.Parse(File.ReadAllText(jsonFile));
            return JsonSerializer.Deserialize<NPCTextFile>(json);
        }

        public static void Write(string jsonFile, NPCTextFile allNPCsText, AppConfig config)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(allNPCsText, jsonOptions);
            File.WriteAllText(jsonFile, json);
            DisplayMessage.Config(config);
            DisplayMessage.TextExtracted(jsonFile);
        }
    }
}