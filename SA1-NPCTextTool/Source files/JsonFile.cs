using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SA1_NPCTextTool
{
    public static class JsonFile
    {
        public static void Create(string filename, JsonContents allNPCsText, AppConfig config)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(allNPCsText, jsonOptions);

            File.WriteAllText(filename, json);
            DisplayMessage.Config(config);
            DisplayMessage.TextExtracted(filename);
        }

        public static JsonContents Read(string filename)
        {
            var json = JsonNode.Parse(File.ReadAllText(filename));
            return JsonSerializer.Deserialize<JsonContents>(json);
        }
    }
}
