using System;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Config
    {
        public static readonly string Path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Galaxy-Swapper-v2-Config";
        public static readonly string[] Directories = { Path, $"{Path}\\DLLS", $"{Path}\\Plugins", $"{Path}\\LOGS" };
        // Cette classe statique Config permet de gérer les configurations de l'application Galaxy Swapper V2 en créant des répertoires nécessaires et en nettoyant les configurations obsolètes.
        public static void Initialize()
        {
            if (File.Exists($"{Path}\\Key.config")) //Old Config Folder
                Directory.Delete(Path, true);

            foreach (string Dir in Directories)
            {
                Directory.CreateDirectory(Dir);
            }
        }
    }
}
