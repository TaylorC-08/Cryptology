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
Random Rand = new Random();
Console.Write("Enter Candidate number: ");
int Num = int.Parse(Console.ReadLine()!);
Console.Write("Enter Security parameter: ");
long s = int.Parse(Console.ReadLine()!);
long val = Num - 1;
long u = 0;
long r = 0;
while (val % 2  == 0)
{
    val = (val / 2);
    u++;
}
r = val;
for (int i = 0; i < s; i++)
{
    long a = Rand.Next(2, (Num - 2));
    long z = ExpoBySquare(a, r, Num);
    if (z != 1 && z != (Num - 1))
    {
        for (int j = 1; j < (u-1); j++)
        {
            z = ExpoBySquare(z, 2, Num);
            if (z != 1)
            {
                Console.Write("P is composite");
                return;
            }
        }
        if (z != (Num - 1))
        {
            Console.Write("P is composite");
            return;
        }
    }
}
Console.Write("P is likely prime");