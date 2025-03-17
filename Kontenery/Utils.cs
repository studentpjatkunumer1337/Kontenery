namespace Kontenery;

public class Utils
{
    public static int ReadInt()
    {
        string read = Console.ReadLine();
        int buf;
        Int32.TryParse(read, out buf);
        return buf;
    }
    
    public static bool ReadBool()
    {
        string read = Console.ReadLine();
        bool buf;
        Boolean.TryParse(read, out buf);
        return buf;
    }
}