using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Settings
    {
        public static Dictionary<string, dynamic> Cache = new Dictionary<string, dynamic>();
        public static readonly string Path = $"{App.Config}\\Settings.json";
        public static JObject Parse { get; set; } = default!;
        public enum Type
        {
            Installtion,
            EpicInstalltion,
            Language,
            RichPresence,
            CloseFortnite,
            KickWarning,
            Reminded,
            CharacterGender,
            BackpackGender,
            HideNsfw,
            ShareStats,
            SortByStats,
            HeroDefinition,
            IsDev
        }
        // Appelle Create() et Populate()
        public static void Initialize()
        {
            if (!IsValid())
                Create();

            Populate();
            Log.Information("Successfully initialized settings");
        }

        private static bool IsValid()
        {
            if (!File.Exists(Path))
                return false;

            if (!Misc.CanEdit(Path))
                return false;

            string Content = File.ReadAllText(Path);

            if (string.IsNullOrEmpty(Content) || !Content.ValidJson())
                return false;

            var parse = JObject.Parse(Content);

            foreach (string Key in Enum.GetNames(typeof(Type)))
            {
                if (parse[Key] == null)
                    return false;
            }

            Parse = parse;
            return true;
        }

        // L'objet Object créé par défaut, est affecté à la propriété statique Parse, permettant ainsi d'avoir accès à ces paramètres au sein de la classe Settings
        // Le contenu de l'objet JObject (converti en chaîne JSON avec Object.ToString()) est écrit dans le fichier spécifié par Path.
        private static void Create()
        {
            var Object = JObject.FromObject(new
            {
                Installtion = EpicGamesLauncher.FortniteInstallation(),
                EpicInstalltion = EpicGamesLauncher.Installation(),
                Language = "EN",
                RichPresence = true,
                CloseFortnite = true,
                KickWarning = true,
                Reminded = string.Empty,
                CharacterGender = true,
                BackpackGender = true,
                HideNsfw = false,
                ShareStats = true,
                SortByStats = true,
                HeroDefinition = true,
                IsDev = false
            });

            Parse = Object;

            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);

                File.WriteAllText(Path, Object.ToString());
                Log.Information($"Created Settings file to {Path}");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to write settings file! Settings will now only be in memory");
            }
        }
        
        // La méthode Populate remplit un cache (Cache) avec des valeurs extraites d'un fichier de paramètres (Parse) et les stocke sous forme de paires clé-valeur. Chaque clé correspond à un membre de l'énumération Type
        public static void Populate()
        {
            try
            {
                if (Cache.Count != 0)
                    Cache.Clear();

                foreach (string Key in Enum.GetNames(typeof(Type)))
                    Cache.Add(Key, Parse[Key].Value<dynamic>());

                Log.Information($"Successfully populated settings cache with {Cache.Count} enum properties");
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, "Failed to populated settings cache");
            }
        }

        // La méthode Read permet de lire une valeur du cache des paramètres (Cache) en fonction d'un membre de l'énumération Type. 
        // Si la valeur n'est pas présente dans le cache, elle tente de recréer et de re-peupler le cache avant de récupérer la valeu
        public static JToken Read(Type Type)
        {
            if (Cache == null || !Cache.ContainsKey($"{Type}"))
            {
                Log.Error($"Settings cache does not contain {Type} attempting to fix this.");
                Create();
                Populate();
            }
            return Cache[Type.ToString()];
        }

        // La méthode Edit permet de modifier un paramètre spécifique dans le cache (Cache) et de mettre à jour le fichier de paramètres (Settings.json) en conséquence. 
        // Si l'écriture dans le fichier échoue, la nouvelle valeur reste uniquement en mémoire
        public static void Edit(Type Key, JToken Value)
        {
            Cache[Key.ToString()] = Value;

            var NewObject = new JObject();
            foreach (var Object in Cache)
            {
                NewObject.Add(Object.Key, Object.Value);
            }

            Parse = NewObject;

            Log.Information($"Set {Key} to {Value}");

            try
            {
                File.WriteAllText(Path, NewObject.ToString());
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to write settings file! New changes will only be in memory");
            }
        }
    }
}
