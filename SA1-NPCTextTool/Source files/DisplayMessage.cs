namespace SA1_NPCTextTool
{
    public static class DisplayMessage
    {
        public static void AboutTool()
        {
            string about = "Drag and drop an NPC text file to the executable to extract text to a json file.\n" +
                "Edit that json and then drag and drop it to the executable to get new NPC text file in \"New files\" directory.\n" +
                "You can add or remove strings in any group. Each group represents a single line with multiple text blocks.\n" +
                "Note that the NPC text file must also be in the same folder.\n" +
                "The tool supports Windows-1251, Windows-1252 and Shift-JIS encodings. Edit AppConfig.json to set the desired encoding.\n\n" +
                "Made by Irregular Zero.\n";
            Console.WriteLine(about);
        }

        public static void TooManyArguments()
        {
            Console.WriteLine("Too many arguments.\n");
        }

        public static void FileNotFound(string file)
        {
            Console.WriteLine($"File {file} not found.\n");
        }

        public static void WrongExtension()
        {
            Console.WriteLine("The file extension is not supported.");
        }

        public static void WrongBinFile()
        {
            Console.WriteLine("It seems the file you've provided is not a Sonic Adventure NPC text file.");
        }

        public static void WrongJsonFile()
        {
            Console.WriteLine("It seems the json file you've provided hasn't been created by this tool.");
        }

        public static void InvalidJson()
        {
            Console.WriteLine("The json file you provided is invalid. Maybe you've made some mistakes editing it. Check in any json validator.");
        }

        public static void FileCreated(string file)
        {
            Console.WriteLine($"File {file} has been successfully created in the \"New files\" folder!\n");
        }

        public static void TextExtracted(string file)
        {
            Console.WriteLine($"The NPC text has been extracted to {file}!");
        }

        public static void Config(AppConfig config)
        {
            Console.WriteLine($"Config settings:\nEncoding - {config.Encoding.EncodingName}\n");
        }
    }
}
