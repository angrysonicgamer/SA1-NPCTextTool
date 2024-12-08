using System.Text;

namespace SA1_NPCTextTool
{
    public static class Program
    {
        private static void SetAppTitle()
        {
            Console.Title = "Sonic Adventure NPC Text Tool";
        }


        public static void Main(string[] args)
        {
            SetAppTitle();
            
            if (args.Length == 0)
            {
                DisplayMessage.AboutTool();
                return;
            }
            
            if (args.Length > 1)
            {
                DisplayMessage.TooManyArguments();
                DisplayMessage.AboutTool();
                return;
            }
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            AppConfig config = new();
            config.Read();

            string sourceFile = args[0];
            string fileExtension = Path.GetExtension(args[0]).ToLower();

            if (!File.Exists(sourceFile))
            {
                DisplayMessage.FileNotFound(sourceFile);
                return;
            }

            // Main actions

            if (fileExtension == ".bin")
            {
                NPCText.Extract(sourceFile, config);
            }
            else if (fileExtension == ".json")
            {
                NPCText.Import(sourceFile, config);
            }
            else
            {
                DisplayMessage.WrongExtension();
            }
        }
    }
}
