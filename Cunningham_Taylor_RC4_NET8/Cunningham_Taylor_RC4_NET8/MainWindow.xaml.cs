using System;
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
using System.Diagnostics.Eventing.Reader;

namespace Cunningham_Taylor_RC4_NET8
{
    /// <summary>
    /// Sources I used to read about some of the functions and methods used throughout the code
    /// https://learn.microsoft.com/en-us/dotnet/api/system.object.tostring?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.convert?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/api/system.convert.tohexstring?view=net-10.0
    /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/types/how-to-convert-between-hexadecimal-strings-and-numeric-types
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators
    /// https://learn.microsoft.com/en-us/dotnet/api/system.uri.ishexdigit?view=net-10.0
    /// </summary>
    public partial class MainWindow : Window
    {
        //Global variables used for encryption and decryption
        string Keyword;
        int len;
        int plaintextLen;
        byte[] Keystreambytes;
        string Keystream;
        String Output;
        byte[] Keybytes;
        byte[] S = new byte[256];
        byte[] Textbytes;
        //Key Scheduling algorithm (KSA) that creates a byte array for PRGA to use in developing the keystream
        public byte[] KSA(byte[] Keybytes)
        {
            for (int i = 0; i<256; i++)
            {
                S[i] = (byte)i;
            }
            int j = 0;
            for (int i = 0; i<256; i++)
            {
                j = ((j + S[i]) + Keybytes[ i % len]) % 256;
                (S[i], S[j]) = (S[j], S[i]);
            }
            return S;
        }
        //Pseudo-random generation algorithm (PRGA) which generates the keystream by swapping elements of array and XORing with the keystream to generate the output
        public void PRGA()
        {
            int i = 0;
            int j = 0;
            for (int n = 0; n < plaintextLen; n++)
            {
                i = (i+1) % 256;
                j = (j + S[i]) % 256;
                (S[i], S[j]) = (S[j], S[i]);
                Keystreambytes[n] += S[(S[i] + S[j]) % 256];
            }
            byte[] outputbytes = new byte[plaintextLen];
            for (int n = 0; n < plaintextLen; n++)
            {
                outputbytes[n] = (byte)(Keystreambytes[n] ^ Textbytes[n]);
            }
            //Outputs in HexString format using the convert class
            Keystream = Convert.ToHexString(Keystreambytes);
            Output = Convert.ToHexString(outputbytes);
        }
        //Function to determine if the typed character is a hex character for the ciphertext in decryption
        private bool IsHex(string text)
        {
            foreach (char c in text)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtPlainE_TextChanged(object sender, TextChangedEventArgs e)
        {
            Keystream = "";
            Output = "";
            S = new byte[256];
            //Takes a key if there is one on either the decrypt or encrypt side of the program
            if (string.IsNullOrEmpty(EncryptKey.Text))
            {
                if (string.IsNullOrEmpty(DecryptKey.Text))
                {
                    return;
                }
                EncryptKey.Text = DecryptKey.Text;
            }
            //Takes the keyword and converts it to bytes and stores the keyword length
            Keyword = EncryptKey.Text.ToString();
            Keybytes = System.Text.Encoding.UTF8.GetBytes(Keyword);
            len = Keyword.Length;
            //Determine keyword length, makes the text to bytes and prepares for KSA and PRGA
            plaintextLen = txtPlainE.Text.Length;
            Keystreambytes = new byte[plaintextLen];
            Textbytes = System.Text.Encoding.UTF8.GetBytes(txtPlainE.Text);
            //Use the KSA and PRGA algorithm and output results in Cipher and Keystream as Hex
            KSA(Keybytes);
            PRGA();
            txtCipherE.Text = Output;
            txtKeystreamE.Text = Keystream;
        }

        private void txtCipherD_TextChanged(object sender, TextChangedEventArgs e)
        {
            Keystream = "";
            Output = "";
            S = new byte[256];
            //Checks for ciphertext to ensure it is a multiple of 2 to prevent errors because I don't use buttons
            if (txtCipherD.Text.Length % 2 != 0)
            {
                return;
            }
            //Takes a key if there is one on either the decrypt or encrypt side of the program
            if (string.IsNullOrEmpty(DecryptKey.Text))
            {
                if (string.IsNullOrEmpty(EncryptKey.Text))
                {
                    return;
                }
                DecryptKey.Text = EncryptKey.Text;
            }
            //Takes the keyword and converts it to bytes and stores the keyword length
            Keyword = DecryptKey.Text.ToString();
            Keybytes = System.Text.Encoding.UTF8.GetBytes(Keyword);
            len = Keyword.Length;
            //Determine keyword length, makes the text to bytes and prepares for KSA and PRGA
            plaintextLen = txtCipherD.Text.Length / 2;
            Keystreambytes = new byte[plaintextLen];
            Textbytes = Convert.FromHexString(txtCipherD.Text);
            //Use the KSA and PRGA algorithm and output results in ASCII for plaintext and Keystream as Hex
            KSA(Keybytes);
            PRGA();
            txtPlainD.Text = System.Text.Encoding.UTF8.GetString(Convert.FromHexString(Output));
            txtKeystreamD.Text = Keystream;
        }
        //Checks Ciphertext input to determine if it is a hexidecimal value
        private void txtCipherD_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Prevents the user from entering non-hex characters using the built in IsHex function, as documented above in the sources
            e.Handled = !IsHex(e.Text);
        }

    }
}