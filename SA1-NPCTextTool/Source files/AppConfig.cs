using System.Text;
using System.Text.Json.Nodes;

namespace SA1_NPCTextTool
{
    public class AppConfig
    {
        public Encoding Encoding { get; set; }

        public void Read()
        {
            var json = JsonNode.Parse(File.ReadAllText("AppConfig.json"));
            var jsonConfig = json["Config"];

            Encodings encoding = Enum.Parse<Encodings>(jsonConfig["Encoding"].GetValue<string>());
            Encoding = Encoding.GetEncoding((int)encoding);
        }
    }
}
