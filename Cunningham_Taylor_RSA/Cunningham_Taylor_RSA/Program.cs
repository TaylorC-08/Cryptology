//Ask the user for encrypt or decryption
Console.Write("Encrypt or Decrypt? (E/D): ");
string mode = Console.ReadLine()!.ToUpper();
string message = "";
long exponent = 0;
if (mode == "E")
{
    Console.Write("Enter Message: ");
    message = Console.ReadLine()!;
    Console.Write("Enter Exponent value: ");
    exponent = long.Parse(Console.ReadLine()!);
} else if (mode == "D")
{
    Console.Write("Enter d value: ");
    exponent = long.Parse(Console.ReadLine()!);
}
Console.Write("Enter N: ");
long modulo = long.Parse(Console.ReadLine()!);
Console.Write("Enter block size in 8 or 16: ");
int Size = int.Parse(Console.ReadLine()!);
static long ExpoBySquare(long Base, long Exponent, long modulo)
{
    long result = 1;
    // mod the base
    Base = Base % modulo;
    while (Exponent > 0)
    {
        // If exponent is odd, multiply current result
        if (Exponent % 2 == 1)
        {
            result = (result * Base) % modulo;
        }
        // Square Base and halve exponent
        Base = (Base * Base) % modulo;
        Exponent = Exponent / 2;
    }
    return result;
}
if (mode == "D")
{
    //Get ciphertext and decrypt
    Console.Write("Enter ciphertext space-separated: ");
    string[] cipher = Console.ReadLine()!.Split(' ');
    Console.WriteLine("\nEncrypted/Decrypted Message:");
    if (Size == 8)
    {
        foreach (string token in cipher)
        {
            long c = long.Parse(token);
            long m = ExpoBySquare(c, exponent, modulo);
            Console.Write((char)m);  // convert number back to character
        }
    }
    else if (Size == 16)
    {
        foreach (string token in cipher)
        {
            long c = long.Parse(token);
            long m = ExpoBySquare(c, exponent, modulo);
            // Split 16-bit block back into two characters
            char c1 = (char)(m / 256);
            char c2 = (char)(m % 256);
            Console.Write(c1);
            if (c2 != '\0') Console.Write(c2);
        }
    }
}
else
{
    //Encrypt
    Console.WriteLine("\nEncrypted/Decrypted Message:");
    if (Size == 8)
    {
        // Encrypts one character at a time
        foreach (char c in message)
        {
            long m = (long)c;
            long result = ExpoBySquare(m, exponent, modulo);
            Console.Write(result + " ");
        }
    }
    else if (Size == 16)
    {
        // Encrypts two characters at a time
        for (int i = 0; i < message.Length; i += 2)
        {
            long m;

            // Combine two ASCII values into one 16-bit
            if (i + 1 < message.Length)
            {
                m = ((long)message[i] * 256) + (long)message[i + 1];
            }
            else
            {
                // Handles odd number of characters
                m = (long)message[i];
            }

            long result = ExpoBySquare(m, exponent, modulo);
            Console.Write(result + " ");
        }
    }
    else
    {
        Console.WriteLine("Invalid block size. Enter 8 or 16.");
    }
}
