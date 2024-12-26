namespace SA1_NPCTextTool
{
    public static class DisplayMessage
    {
        public static void AboutTool()
        {
            string about = "This is a tool to work with town NPC text in Sonic Adventure (any version).\n\n" +
                "- Usage -\n" +
                "Drag and drop an NPC text file to the executable to extract text to a json file.\n" +
                "Edit that json and then drag and drop it to the executable to get new NPC text file in \"New files\" subfolder.\n" +
                "Note that the NPC text file must also be in the same folder as json.\n\n" +
                "- CMD usage -\n" +
                "SA1-NPCTextTool filename\n\n" +
                "- Editing extracted data -\n" +
                "You can add or remove strings in any group.\n" +
                "Each group represents a single line with multiple text blocks.\n" +
                "The tool assumes all text strings would use \"block\" centering method (an imported text will start with '\\a').\n" +
                "You can use \"each line\" centering by adding '\\t' to the beginning of a string in a json file.\n\n" +
                "The tool supports Windows-1251, Windows-1252 and Shift-JIS encodings.\n" +
                "Edit AppConfig.json to set the desired encoding.\n";
            Console.WriteLine(about);
            Wait();
        }

        public static void TooManyArguments()
        {
            Console.WriteLine("Too many arguments.\n");
        }

        public static void FileNotFound(string file)
        {
            Console.WriteLine($"File {file} not found.\n");
            Wait();
        }

        public static void WrongExtension()
        {
            Console.WriteLine("The file extension is not supported.\n");
            Wait();
        }

        public static void WrongBinFile()
        {
            Console.WriteLine("It seems the file you've provided is not a Sonic Adventure NPC text file.\n");
            Wait();
        }

        public static void WrongJsonFile()
        {
            Console.WriteLine("It seems the json file you've provided hasn't been created by this tool.\n");
            Wait();
        }

        public static void InvalidJson()
        {
            Console.WriteLine("The json file you provided is invalid. Maybe you've made some mistakes editing it. Check in any json validator.\n");
            Wait();
        }

        public static void NPCFileCreated(string file)
        {
            Console.WriteLine($"NPC text file {file} has been successfully created in the \"New files\" folder!\n");
            Wait();
        }

        public static void TextExtracted(string file)
        {
            Console.WriteLine($"The NPC text has been extracted to {file}!\n");
            Wait();
        }

        public static void ReadingFile(string file)
        {
            Console.WriteLine($"Reading file: {Path.GetFileName(file)}...\n");
        }

        public static void Config(AppConfig config)
        {
            Console.WriteLine($"Config settings:" +
                $"\nEncoding - {config.Encoding.EncodingName}\n");
        }


        private static void Wait()
        {
            Console.WriteLine("Press Enter to exit");

            while (true)
            {
                var keyPressed = Console.ReadKey(true).Key;
                if (keyPressed == ConsoleKey.Enter) break;
            }
        }
    }
}
