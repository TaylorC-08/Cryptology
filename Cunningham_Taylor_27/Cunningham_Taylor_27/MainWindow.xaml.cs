using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace Cunningham_Taylor_27
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //SubBytes lookup table
        public string[,] SubBytes =
{
                { "63", "7c", "77", "7b", "f2", "6b", "6f", "c5", "30", "01", "67", "2b", "fe", "d7", "ab", "76" },
                { "ca", "82", "c9", "7d", "fa", "59", "47", "f0", "ad", "d4", "a2", "af", "9c", "a4", "72", "c0" },
                { "b7", "fd", "93", "26", "36", "3f", "f7", "cc", "34", "a5", "e5", "f1", "71", "d8", "31", "15" },
                { "04", "c7", "23", "c3", "18", "96", "05", "9a", "07", "12", "80", "e2", "eb", "27", "b2", "75" },
                { "09", "83", "2c", "1a", "1b", "6e", "5a", "a0", "52", "3b", "d6", "b3", "29", "e3", "2f", "84" },
                { "53", "d1", "00", "ed", "20", "fc", "b1", "5b", "6a", "cb", "be", "39", "4a", "4c", "58", "cf" },
                { "d0", "ef", "aa", "fb", "43", "4d", "33", "85", "45", "f9", "02", "7f", "50", "3c", "9f", "a8" },
                { "51", "a3", "40", "8f", "92", "9d", "38", "f5", "bc", "b6", "da", "21", "10", "ff", "f3", "d2" },
                { "cd", "0c", "13", "ec", "5f", "97", "44", "17", "c4", "a7", "7e", "3d", "64", "5d", "19", "73" },
                { "60", "81", "4f", "dc", "22", "2a", "90", "88", "46", "ee", "b8", "14", "de", "5e", "0b", "db" },
                { "e0", "32", "3a", "0a", "49", "06", "24", "5c", "c2", "d3", "ac", "62", "91", "95", "e4", "79" },
                { "e7", "c8", "37", "6d", "8d", "d5", "4e", "a9", "6c", "56", "f4", "ea", "65", "7a", "ae", "08" },
                { "ba", "78", "25", "2e", "1c", "a6", "b4", "c6", "e8", "dd", "74", "1f", "4b", "bd", "8b", "8a" },
                { "70", "3e", "b5", "66", "48", "03", "f6", "0e", "61", "35", "57", "b9", "86", "c1", "1d", "9e" },
                { "e1", "f8", "98", "11", "69", "d9", "8e", "94", "9b", "1e", "87", "e9", "ce", "55", "28", "df" },
                { "8c", "a1", "89", "0d", "bf", "e6", "42", "68", "41", "99", "2d", "0f", "b0", "54", "bb", "16" }
            };
        //Function to determine if the typed character is a hex character
        private bool IsHex(string text)
        {
            foreach (char c in text)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }
        //RotWord function to shift characters
        string RotWord(string text, int n)
        {
            n %= text.Length;
            return text.Substring(n) + text.Substring(0, n);
        }
        //SubWord function to get the subword from lookup table
        String SubWord(string text)
        {
            //Take the current string and convert to bytes
            byte[] bytes = Convert.FromHexString(text);
            //Create a stringbuilder and use the lookup table SubBytes
            StringBuilder SubWord = new StringBuilder();
            foreach (byte b in bytes)
            {
                //Determine the row and column to look up in the SubBytes table
                int row = (b >> 4) & 0x0F;
                int col = b & 0x0F;
                //Lookup annd append to the string
                SubWord.Append(SubBytes[row, col]);
            }
            return SubWord.ToString();
        }
        byte GFRCON(byte b)
        {
            //XOR if the item is too high
            byte GFRcon = (byte)(b << 1);
            if ((b & 0x80) != 0)
            {
                GFRcon ^= 0x1b;
            }
            return GFRcon;
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtOutput.Text = "";
            //Error checks
            byte Rcon = 0x01;
            String Input = txtInput.Text.ToString();
            if (string.IsNullOrEmpty(Input))
            {
                txtInput.Text = "";
                return;
            }
            //Get input text to bytes for error checking
            string binary = string.Join("", Input.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            if (binary.Length != 128)
            {
                txtOutput.Text = "Key is not 128 bits";
                return;
            }
            //Calculate w0-w3
            String currentW = "";
            for (int i = 0; i < Input.Length/8; i ++)
            {
                currentW = Input.Substring(i*8, 8);
                txtOutput.Text += "w" + (i).ToString() + ": "+ currentW.ToString() + "\n";
            }
            String w4 = "";
            //calculate w4

            //Start with Rotword & output
            w4 = RotWord(currentW, 2);
            txtOutput.Text += "After RotWord(): " + w4 + "\n";

            //Use SubWord and output
            w4 = SubWord(w4);
            txtOutput.Text += "After SubWord(): " + w4 + "\n";

            //Final steps for calculating the final w4
            byte[] bytes = Convert.FromHexString(w4);
            w4 = "";
            bytes[0] ^= Rcon;
            Rcon = GFRCON(Rcon);
            for (int i = 0; i < bytes.Length; i++)
            {
                w4 += bytes[i].ToString("x2");
            }
            txtOutput.Text += "After XOR with RCON: " + w4;
            String w0 = Input.Substring(0, 8);
            byte[] w0bytes = Convert.FromHexString(w0);
            byte[] w4bytes = Convert.FromHexString(w4);

            w4 = "";
            for (int i = 0; i < w4bytes.Length; i++)
            {
                w4 += (w4bytes[i] ^ w0bytes[i]).ToString("x2");
            }
            txtOutput.Text += "\nw4: " + w4;
        }

        private void txtInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Prevents the user from entering non-hex characters using the built in IsHex function, as documented above in the sources
            e.Handled = !IsHex(e.Text);
        }
    }
}