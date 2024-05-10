using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1.Layers.Files
{
    public class clsSaveData
    {
        public static bool CreateFolderOfLayers(string FolderPath)
        {
            if (!Directory.Exists(FolderPath))
            {
                try
                {
                    Directory.CreateDirectory(FolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Creating Folder " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return true;
        }

        public static bool SaveToFile(string Data, string FileName, string FolderPath)
        {
            if (!CreateFolderOfLayers(FolderPath))
            {
                return false;
            }

            try
            {

                File.WriteAllText($"{FolderPath}\\{FileName}.txt", Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Data Not Saved Successffully " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return true;
        }
    }
}
