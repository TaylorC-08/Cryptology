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

namespace Cunningham_Taylor_FINAL
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        //ROTR function to circular shift characters
        //The rotate right (circular right shift) operation, where x is a w-bit word and n is an integer
        private ulong ROTR(ulong x, int n)
        {
            return (x >> n) | (x << (64 - n));
        }
        //SHR to shift characters
        //The right shift operation, where x is a w-bit word and n is an integer
        private ulong SHR(ulong x, int n)
        {
            return x >> n;
        }

        private void txtIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtOut.Text = "";
            String Input = txtIn.Text.ToString();
            if (string.IsNullOrEmpty(Input))
            {
                txtIn.Text = "";
                return;
            }
            // Parse hex string into a 64-bit unsigned integer
            if (!ulong.TryParse(Input.PadLeft(16, '0'),null, out ulong x))
            {
                txtOut.Text = "Invalid hex input.";
                return;
            }
            ulong output = ROTR(x, 19) ^ ROTR(x, 61) ^ SHR(x, 6);
            txtOut.Text = $"0x{output:X16}";
        }
        private void txtIn_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Prevents the user from entering non-hex characters using the built in IsHex function, as documented above in the sources
            e.Handled = !IsHex(e.Text);
        }
    }
}