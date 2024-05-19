using Microsoft.Win32;
using System;
using System.Text;
using System.Windows.Forms;

public class LoginByRegistry
{
    public static bool WritingLoginInfo(string username, string password, string folderName)
    {
        string path = "HKEY_CURRENT_USER\\SOFTWARE\\" + folderName.Trim();
        try
        {
            Registry.SetValue(path, username, password, RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
            return false;
        }
        return true;
    }

    public static bool ReadingLoginInfo(string username, string password, string folderName)
    {
        string path = "HKEY_CURRENT_USER\\SOFTWARE\\" + folderName.Trim();
        try
        {
            return password == (Registry.GetValue(path, username, null) as string);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
