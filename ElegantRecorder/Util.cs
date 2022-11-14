using System.Windows.Forms;
using System.Linq;
using System.IO;
using System;

namespace ElegantRecorder
{
    public static class Util
    {
        public static string HotkeyToString(int hotkey)
        {
            return string.Join("+", ((Keys)hotkey).ToString().Split(", ").Reverse());
        }

        public static string ReadUntil(FileStream Stream, string UntilText)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            System.Text.StringBuilder returnTextBuilder = new System.Text.StringBuilder();
            string returnText = string.Empty;
            int size = Convert.ToInt32(UntilText.Length / (double)2) - 1;
            byte[] buffer = new byte[size + 1];
            int currentRead = -1;

            while (currentRead != 0)
            {
                string collected = null;
                string chars = null;
                int foundIndex = -1;

                currentRead = Stream.Read(buffer, 0, buffer.Length);
                chars = System.Text.Encoding.Default.GetString(buffer, 0, currentRead);

                builder.Append(chars);
                returnTextBuilder.Append(chars);

                collected = builder.ToString();
                foundIndex = collected.IndexOf(UntilText);

                if (foundIndex >= 0)
                {
                    returnText = returnTextBuilder.ToString();

                    int indexOfSep = returnText.IndexOf(UntilText);
                    int cutLength = returnText.Length - indexOfSep;

                    returnText = returnText.Remove(indexOfSep, cutLength);

                    builder.Remove(0, foundIndex + UntilText.Length);

                    if (cutLength > UntilText.Length)
                        Stream.Position -= cutLength - UntilText.Length;

                    return returnText;
                }
                else if (!collected.Contains(UntilText.First()))
                    builder.Length = 0;
            }

            return string.Empty;
        }
    }
}
