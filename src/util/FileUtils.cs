using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fountain
{
    /// <summary>
    /// Contains generic utilities relating to files.
    /// </summary>
    public static class FileUtils
    {

        public static string GetFirstDrive()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
if (drives.Length > 0)
{
    string firstDriveName = drives[0].Name;
    return firstDriveName;
}
return null;
        }


        public static void ReadAllFilesRecursively(string directory, string[] excludeDirectories = null, string[] includeTypes = null, Action<string, string, string> action = null, Action onAllComplete = null)
        {
            // Get all files in the directory
            string[] files = Directory.GetFiles(directory);

            // Read the contents of each file and print it to the console
            foreach (string file in files)
            {
                string filePath = Path.GetFullPath(file);
                string fileName = Path.GetFileName(file);
                string fileContents = File.ReadAllText(file);
                string fileType = !fileName.Contains(".") ? null : "." + fileName.Split(".")[1];
                if (includeTypes != null) if (!includeTypes.ToList().Contains(fileType)) continue;
                if (fileName == null) continue;
                action?.Invoke(filePath, fileName, fileContents);
            }

            // Get all subdirectories in the directory
            string[] subdirectories = Directory.GetDirectories(directory);

            // Recursively read all files in each subdirectory
            foreach (string subdirectory in subdirectories)
{
    string dirName = Path.GetFileName(subdirectory);
    if (excludeDirectories != null) if (excludeDirectories.ToList().Contains(dirName)) continue;
    ReadAllFilesRecursively(subdirectory, excludeDirectories, includeTypes, action);
}
onAllComplete?.Invoke();

        }
    }
}