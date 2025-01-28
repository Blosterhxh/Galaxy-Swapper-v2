﻿using Galaxy_Swapper_v2.Workspace.Structs;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Binaries
    {
        public static readonly string Path = App.Config + "\\Binaries";

        // Empreinte de hachage, nom du fichier binaire, chemi d'accès au fichier incorporé
        private static readonly List<BinaryData> BinaryData = new List<BinaryData>
        {
            //new BinaryData("8A06BB79F400AEC933CD8BADD5B63DF54DA5B02B95CF7C0EA29E0CDB40F72E6E", "oo2core_5_win64.dll", "Galaxy_Swapper_v2.Workspace.Assets.oo2core_5_win64.dll"), - No longer in use
            new BinaryData("6F5D41A7892EA6B2DB420F2458DAD2F84A63901C9A93CE9497337B16C195F457", "oo2core_9_win64.dll", "Galaxy_Swapper_v2.Workspace.Assets.oo2core_9_win64.dll"),
        };

        // L'objectif de Initialize() est de :
        // Configurer le chemin des bibliothèques dynamiques (DLL).
        // Vérifier l'existence, l'intégrité et la disponibilité des fichiers binaires.
        // Si nécessaire, supprimer et régénérer les fichiers à partir des ressources intégrées
        public static void Initialize()
        {
            Win32.SetDllDirectory(Path);

            foreach (BinaryData binarydata in BinaryData)
            {
                string path = Path + "\\" + binarydata.Name;
                if (File.Exists(path))
                {
                    if (!path.CanEdit())
                    {
                        Log.Warning(path + " is currently in use and we can't verify It's hash");
                        continue;
                    }

                    if (Misc.Hash(path) == binarydata.Hash)
                    {
                        Log.Information($"{path} hash OK");
                        continue;
                    }

                    Log.Warning(path + " hash miss matched! Attempting to remove");
                    File.Delete(path);
                }

                Log.Information("Attempting to generate " + binarydata.Name + " from resource " + binarydata.Path);

                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(binarydata.Path);
                using FileStream destination = new FileStream(path, FileMode.Create);

                try
                {
                    stream.CopyTo(destination);
                    stream.Close();
                    destination.Close();
                }
                catch (Exception exception)
                {
                    Log.Fatal("Failed to write resource from " + binarydata.Path + " to " + path);
                    throw;
                }
            }
        }
    }
}
