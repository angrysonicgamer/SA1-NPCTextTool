using System.Text;

namespace SA1_NPCTextTool
{
    public static class Extensions
    {
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
    }
}
