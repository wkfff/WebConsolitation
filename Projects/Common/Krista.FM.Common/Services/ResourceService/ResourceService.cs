﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Krista.FM.Common.Services
{
    /// <summary>
    /// Класс содержит два менеджера ресуксов, которые обрабатываею строковые ресурсы
    /// и ресурсы иконок (изображений) для приложения.
    /// </summary>
    public static class ResourceService
    {
        const string uiLanguageProperty = "CoreProperties.UILanguage";

        const string stringResources = "StringResources";
        const string imageResources = "BitmapResources";

        static string resourceDirectory;

        public static void InitializeService(string resourceDirectory)
        {
            if (ResourceService.resourceDirectory != null)
                throw new InvalidOperationException("Service is already initialized.");
            if (resourceDirectory == null)
                throw new ArgumentNullException("resourceDirectory");

            ResourceService.resourceDirectory = resourceDirectory;

            //PropertyService.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChange);
            LoadLanguageResources(Thread.CurrentThread.CurrentUICulture.Name);
        }

        /*public static string Language
        {
            get
            {
                return PropertyService.Get(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
            }
            set
            {
                PropertyService.Set(uiLanguageProperty, value);
            }
        }*/

        /// <summary>English strings (list of resource managers)</summary>
        static List<ResourceManager> strings = new List<ResourceManager>();
        /// <summary>Neutral/English images (list of resource managers)</summary>
        static List<ResourceManager> icons = new List<ResourceManager>();

        /// <summary>Hashtable containing the local strings from the main application.</summary>
        static Hashtable localStrings = null;
        static Hashtable localIcons = null;

        static Dictionary<string, Icon> iconCache = new Dictionary<string, Icon>();
        static Dictionary<string, Bitmap> bitmapCache = new Dictionary<string, Bitmap>();
        static Dictionary<string, Stream> resourceStreamCache = new Dictionary<string, Stream>();

        /// <summary>Strings resource managers for the current language</summary>
        static List<ResourceManager> localStringsResMgrs = new List<ResourceManager>();
        /// <summary>Image resource managers for the current language</summary>
        static List<ResourceManager> localIconsResMgrs = new List<ResourceManager>();

        /// <summary>List of ResourceAssembly</summary>
        static List<ResourceAssembly> resourceAssemblies = new List<ResourceAssembly>();

        class ResourceAssembly
        {
            Assembly assembly;
            string baseResourceName;
            bool isIcons;

            public ResourceAssembly(Assembly assembly, string baseResourceName, bool isIcons)
            {
                this.assembly = assembly;
                this.baseResourceName = baseResourceName;
                this.isIcons = isIcons;
            }

            ResourceManager TrySatellite(string language)
            {
                // ResourceManager should automatically use satellite assemblies, but it doesn't work
                // and we have to do it manually.
                string fileName = Path.GetFileNameWithoutExtension(assembly.Location) + ".resources.dll";
                fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(assembly.Location), language), fileName);
                if (File.Exists(fileName))
                {
                    Trace.TraceInformation("Loging resources " + baseResourceName + " loading from satellite " + language);
                    return new ResourceManager(baseResourceName, Assembly.LoadFrom(fileName));
                }
                else
                {
                    return null;
                }
            }

            public void Load()
            {
                string logMessage = "Loading resources " + baseResourceName + "." + currentLanguage + ": ";
                ResourceManager manager = null;
                
                if (assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage + ".resources") != null)
                {
                    Trace.TraceInformation(logMessage + " loading from main assembly");
                    manager = new ResourceManager(baseResourceName + "." + currentLanguage, assembly);
                }
                else if (currentLanguage.IndexOf('-') > 0
                           && assembly.GetManifestResourceInfo(baseResourceName + "." + currentLanguage.Split('-')[0] + ".resources") != null)
                {
                    Trace.TraceInformation(logMessage + " loading from main assembly (no country match)");
                    manager = new ResourceManager(baseResourceName + "." + currentLanguage.Split('-')[0], assembly);
                }
                else
                {
                    // try satellite assembly
                    manager = TrySatellite(currentLanguage);
                    if (manager == null && currentLanguage.IndexOf('-') > 0)
                    {
                        manager = TrySatellite(currentLanguage.Split('-')[0]);
                    }
                }
                if (manager == null)
                {
                    Trace.TraceWarning(logMessage + "NOT FOUND");
                }
                else
                {
                    if (isIcons)
                        localIconsResMgrs.Add(manager);
                    else
                        localStringsResMgrs.Add(manager);
                }
            }
        }

        /// <summary>
        /// Registers string resources in the resource service.
        /// </summary>
        /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
        /// <param name="assembly">The assembly which contains the resource file.</param>
        /// <example><c>ResourceService.RegisterStrings("TestAddin.Resources.StringResources", GetType().Assembly);</c></example>
        public static void RegisterStrings(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralStrings(new ResourceManager(baseResourceName, assembly));
            ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, false);
            resourceAssemblies.Add(ra);
            ra.Load();
        }

        public static void RegisterNeutralStrings(ResourceManager stringManager)
        {
            strings.Add(stringManager);
        }

        /// <summary>
        /// Registers image resources in the resource service.
        /// </summary>
        /// <param name="baseResourceName">The base name of the resource file embedded in the assembly.</param>
        /// <param name="assembly">The assembly which contains the resource file.</param>
        /// <example><c>ResourceService.RegisterImages("TestAddin.Resources.BitmapResources", GetType().Assembly);</c></example>
        public static void RegisterImages(string baseResourceName, Assembly assembly)
        {
            RegisterNeutralImages(new ResourceManager(baseResourceName, assembly));
            ResourceAssembly ra = new ResourceAssembly(assembly, baseResourceName, true);
            resourceAssemblies.Add(ra);
            ra.Load();
        }

        public static void RegisterNeutralImages(ResourceManager imageManager)
        {
            icons.Add(imageManager);
        }

        /*static void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.Key == uiLanguageProperty && e.NewValue != e.OldValue)
            {
                LoadLanguageResources((string)e.NewValue);
                if (LanguageChanged != null)
                    LanguageChanged(null, e);
            }
        }*/

        static string currentLanguage;

        static void LoadLanguageResources(string language)
        {
            iconCache.Clear();
            bitmapCache.Clear();

            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            }
            catch (Exception)
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
                }
                catch (Exception) { }
            }

            localStrings = Load(stringResources, language);
            if (localStrings == null && language.IndexOf('-') > 0)
            {
                localStrings = Load(stringResources, language.Split('-')[0]);
            }

            localIcons = Load(imageResources, language);
            if (localIcons == null && language.IndexOf('-') > 0)
            {
                localIcons = Load(imageResources, language.Split('-')[0]);
            }

            localStringsResMgrs.Clear();
            localIconsResMgrs.Clear();
            currentLanguage = language;
            foreach (ResourceAssembly ra in resourceAssemblies)
            {
                ra.Load();
            }
        }

        #region Font loading
        static Font defaultMonospacedFont;

        public static Font DefaultMonospacedFont
        {
            get
            {
                if (defaultMonospacedFont == null)
                {
                    defaultMonospacedFont = LoadDefaultMonospacedFont(FontStyle.Regular);
                }
                return defaultMonospacedFont;
            }
        }

        /// <summary>
        /// Loads the default monospaced font (Consolas or Courier New).
        /// </summary>
        public static Font LoadDefaultMonospacedFont(FontStyle style)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                && Environment.OSVersion.Version.Major >= 6)
            {
                return LoadFont("Consolas", 10, style);
            }
            else
            {
                return LoadFont("Courier New", 10, style);
            }
        }

        /// <summary>
        /// The LoadFont routines provide a safe way to load fonts.
        /// </summary>
        /// <param name="fontName">The name of the font to load.</param>
        /// <param name="size">The size of the font to load.</param>
        /// <returns>
        /// The font to load or the menu font, if the requested font couldn't be loaded.
        /// </returns>
        public static Font LoadFont(string fontName, int size)
        {
            return LoadFont(fontName, size, FontStyle.Regular);
        }

        /// <summary>
        /// The LoadFont routines provide a safe way to load fonts.
        /// </summary>
        /// <param name="fontName">The name of the font to load.</param>
        /// <param name="size">The size of the font to load.</param>
        /// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
        /// <returns>
        /// The font to load or the menu font, if the requested font couldn't be loaded.
        /// </returns>
        public static Font LoadFont(string fontName, int size, FontStyle style)
        {
            try
            {
                return new Font(fontName, size, style);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                return SystemInformation.MenuFont;
            }
        }

        /// <summary>
        /// The LoadFont routines provide a safe way to load fonts.
        /// </summary>
        /// <param name="fontName">The name of the font to load.</param>
        /// <param name="size">The size of the font to load.</param>
        /// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
        /// <returns>
        /// The font to load or the menu font, if the requested font couldn't be loaded.
        /// </returns>
        public static Font LoadFont(string fontName, int size, GraphicsUnit unit)
        {
            return LoadFont(fontName, size, FontStyle.Regular, unit);
        }

        /// <summary>
        /// The LoadFont routines provide a safe way to load fonts.
        /// </summary>
        /// <param name="fontName">The name of the font to load.</param>
        /// <param name="size">The size of the font to load.</param>
        /// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
        /// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
        /// <returns>
        /// The font to load or the menu font, if the requested font couldn't be loaded.
        /// </returns>
        public static Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit)
        {
            try
            {
                return new Font(fontName, size, style, unit);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                return SystemInformation.MenuFont;
            }
        }

        /// <summary>
        /// The LoadFont routines provide a safe way to load fonts.
        /// </summary>
        /// <param name="baseFont">The existing font from which to create the new font.</param>
        /// <param name="newStyle">The new style of the font.</param>
        /// <returns>
        /// The font to load or the baseFont (if the requested font couldn't be loaded).
        /// </returns>
        public static Font LoadFont(Font baseFont, FontStyle newStyle)
        {
            try
            {
                return new Font(baseFont, newStyle);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                return baseFont;
            }
        }
        #endregion

        static Hashtable Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                Hashtable resources = new Hashtable();
                ResourceReader rr = new ResourceReader(fileName);
                foreach (DictionaryEntry entry in rr)
                {
                    resources.Add(entry.Key, entry.Value);
                }
                rr.Close();
                return resources;
            }
            return null;
        }

        static Hashtable Load(string name, string language)
        {
            return Load(resourceDirectory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
        }

        /// <summary>
        /// Returns a string from the resource database, it handles localization
        /// transparent for the user.
        /// </summary>
        /// <returns>
        /// The string in the (localized) resource database.
        /// </returns>
        /// <param name="name">
        /// The name of the requested resource.
        /// </param>
        /// <exception cref="ResourceNotFoundException">
        /// Is thrown when the GlobalResource manager can't find a requested resource.
        /// </exception>
        public static string GetString(string name)
        {
            if (localStrings != null && localStrings[name] != null)
            {
                return localStrings[name].ToString();
            }

            string s = null;
            foreach (ResourceManager resourceManger in localStringsResMgrs)
            {
                try
                {
                    s = resourceManger.GetString(name);
                }
                catch (Exception) { }

                if (s != null)
                {
                    break;
                }
            }

            if (s == null)
            {
                foreach (ResourceManager resourceManger in strings)
                {
                    try
                    {
                        s = resourceManger.GetString(name);
                    }
                    catch (Exception) { }

                    if (s != null)
                    {
                        break;
                    }
                }
            }
            if (s == null)
            {
                throw new ResourceNotFoundException("string >" + name + "<");
            }

            return s;
        }

        static object GetImageResource(string name)
        {
            object iconobj = null;
            if (localIcons != null && localIcons[name] != null)
            {
                iconobj = localIcons[name];
            }
            else
            {
                foreach (ResourceManager resourceManger in localIconsResMgrs)
                {
                    iconobj = resourceManger.GetObject(name);
                    if (iconobj != null)
                    {
                        break;
                    }
                }

                if (iconobj == null)
                {
                    foreach (ResourceManager resourceManger in icons)
                    {
                        try
                        {
                            iconobj = resourceManger.GetObject(name);
                        }
                        catch (Exception) { }

                        if (iconobj != null)
                        {
                            break;
                        }
                    }
                }
            }
            return iconobj;
        }

        /// <summary>
        /// Returns a icon from the resource database, it handles localization
        /// transparent for the user. In the resource database can be a bitmap
        /// instead of an icon in the dabase. It is converted automatically.
        /// </summary>
        /// <returns>
        /// The icon in the (localized) resource database, or null, if the icon cannot
        /// be found.
        /// </returns>
        /// <param name="name">
        /// The name of the requested icon.
        /// </param>
        public static Icon GetIcon(string name)
        {
            lock (iconCache)
            {
                Icon ico;
                if (iconCache.TryGetValue(name, out ico))
                    return ico;

                object iconobj = GetImageResource(name);
                if (iconobj == null)
                {
                    return null;
                }
                if (iconobj is Icon)
                {
                    ico = (Icon)iconobj;
                }
                else
                {
                    ico = Icon.FromHandle(((Bitmap)iconobj).GetHicon());
                }
                iconCache[name] = ico;
                return ico;
            }
        }

        /// <summary>
        /// Returns a bitmap from the resource database, it handles localization
        /// transparent for the user.
        /// </summary>
        /// <returns>
        /// The bitmap in the (localized) resource database.
        /// </returns>
        /// <param name="name">
        /// The name of the requested bitmap.
        /// </param>
        /// <exception cref="ResourceNotFoundException">
        /// Is thrown when the GlobalResource manager can't find a requested resource.
        /// </exception>
        public static Bitmap GetBitmap(string name)
        {
            lock (bitmapCache)
            {
                Bitmap bmp;
                if (bitmapCache.TryGetValue(name, out bmp))
                    return bmp;
                bmp = (Bitmap)GetImageResource(name);
                if (bmp == null)
                {
                    throw new ResourceNotFoundException(name);
                }
                bitmapCache[name] = bmp;
                return bmp;
            }
        }

        /// <summary>
        /// Возвращает поток ресурса из базы данных ресурсов.
        /// </summary>
        /// <param name="name">Имя запрашиваемого ресурса.</param>
        /// <returns>Поток ресурса из базы данных ресурсов.</returns>
        /// <exception cref="ResourceNotFoundException">
        /// Is thrown when the GlobalResource manager can't find a requested resource.
        /// </exception>
        public static Stream GetResourceStream(string name)
        {
            lock (resourceStreamCache)
            {
                Stream stream;
                if (resourceStreamCache.TryGetValue(name, out stream))
                    return stream;
                stream = (Stream)GetImageResource(name);
                if (stream == null)
                {
                    throw new ResourceNotFoundException(name);
                }
                resourceStreamCache[name] = stream;
                return stream;
            }
        }
    }
}
