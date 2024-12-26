using System.Text;

namespace SA1_NPCTextTool
{
    public static class Extensions
    {
        // BinaryReader
        
        public static void SetPosition(this BinaryReader reader, long position)
        {
            reader.BaseStream.Position = position;
        }

        public static T ReadAt<T>(this BinaryReader reader, long position, Func<BinaryReader, T> func)
        {
            var origPosition = reader.BaseStream.Position;

            if (origPosition != position)
            {
                reader.SetPosition(position);
            }

            T value;

            try
            {
                value = func(reader);
            }
            finally
            {
                reader.SetPosition(origPosition);
            }

            return value;
        }

        public static string ReadCString(this BinaryReader reader, Encoding encoding)
        {
            List<byte> textBytes = new List<byte>();

            while (true)
            {
                byte b = reader.ReadByte();
                if (b == 0) break;

                textBytes.Add(b);
            }

            return encoding.GetString(textBytes.ToArray());
        }


        // BinaryWriter

        public static void SetPosition(this BinaryWriter writer, long position)
        {
            writer.BaseStream.Position = position;
        }
    }
}
