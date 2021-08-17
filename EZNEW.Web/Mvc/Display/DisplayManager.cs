using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Linq;
using EZNEW.Web.Mvc.Display.Configuration;
using EZNEW.Expressions;
using EZNEW.Serialization;

namespace EZNEW.Web.Mvc.Display
{
    /// <summary>
    /// Display manager
    /// </summary>
    public static class DisplayManager
    {
        /// <summary>
        /// Property displays
        /// </summary>
        public static readonly Dictionary<string, DisplayText> PropertyDisplays = new Dictionary<string, DisplayText>();

        /// <summary>
        /// Set property display
        /// </summary>
        /// <param name="modelType">model type</param>
        /// <param name="propertyName">property name</param>
        /// <param name="displayName">display name</param>
        public static void SetPropertyDisplay(Type modelType, string propertyName, string displayName)
        {
            if (modelType == null)
            {
                return;
            }
            SetPropertyDisplay(modelType.GUID, propertyName, displayName);
        }

        /// <summary>
        /// Set property display
        /// </summary>
        /// <typeparam name="TModel">model type</typeparam>
        /// <param name="property">property</param>
        /// <param name="displayName">display name</param>
        public static void SetPropertyDisplay<TModel>(Expression<Func<TModel, dynamic>> property, string displayName)
        {
            SetPropertyDisplay(typeof(TModel), ExpressionHelper.GetExpressionText(property), displayName);
        }

        /// <summary>
        /// Configure model type display
        /// </summary>
        /// <param name="displayConfigurationCollection">Model type display configurations</param>
        public static void Configure(DisplayConfigurationCollection displayConfigurationCollection)
        {
            if (displayConfigurationCollection?.Types.IsNullOrEmpty() ?? true)
            {
                return;
            }
            foreach (var typeDisplay in displayConfigurationCollection.Types)
            {
                if (typeDisplay == null || string.IsNullOrWhiteSpace(typeDisplay.ModelTypeFullName) || typeDisplay.Propertys.IsNullOrEmpty())
                {
                    continue;
                }
                Type modelType = Type.GetType(typeDisplay.ModelTypeFullName);
                if (modelType == null)
                {
                    continue;
                }
                foreach (var propertyDisplay in typeDisplay.Propertys)
                {
                    if (propertyDisplay == null || string.IsNullOrWhiteSpace(propertyDisplay.Name) || propertyDisplay.Display == null)
                    {
                        continue;
                    }
                    SetPropertyDisplay(modelType, propertyDisplay.Name, propertyDisplay.Display.DisplayName);
                }
            }
        }

        /// <summary>
        /// Configure display from json
        /// </summary>
        /// <param name="json">json data</param>
        public static void ConfigureByJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            DisplayConfigurationCollection typeConfigurationCollection = JsonSerializer.Deserialize<DisplayConfigurationCollection>(json);
            Configure(typeConfigurationCollection);
        }

        /// <summary>
        /// Configure validation by config file
        /// </summary>
        /// <param name="configDirectoryPath">Config directory path</param>
        /// <param name="fileExtension">Config file extension</param>
        public static void ConfigureByConfigFile(string configDirectoryPath, string fileExtension = "disconfig")
        {
            if (string.IsNullOrWhiteSpace(configDirectoryPath) || string.IsNullOrWhiteSpace(fileExtension) || !Directory.Exists(configDirectoryPath))
            {
                return;
            }
            var files = Directory.GetFiles(configDirectoryPath).Where(c => string.Equals(Path.GetExtension(c).Trim('.'), fileExtension, StringComparison.OrdinalIgnoreCase));
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                ConfigureByJson(json);
            }
            var childDirectorys = new DirectoryInfo(configDirectoryPath).GetDirectories();
            foreach (var directory in childDirectorys)
            {
                ConfigureByConfigFile(directory.FullName, fileExtension);
            }
        }

        /// <summary>
        /// Get the property display
        /// </summary>
        /// <param name="modelType">model type</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static DisplayText GetPropertyDisplay(Type modelType, string propertyName)
        {
            if (modelType == null)
            {
                return null;
            }
            return GetPropertyDisplay(modelType.GUID, propertyName);
        }

        /// <summary>
        /// Get the property display
        /// </summary>
        /// <param name="modelTypeId">model type id</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static DisplayText GetPropertyDisplay(Guid modelTypeId, string propertyName)
        {
            string displayKey = GetDisplayConfigKey(modelTypeId, propertyName);
            if (PropertyDisplays.ContainsKey(displayKey))
            {
                return PropertyDisplays[displayKey];
            }
            return null;
        }

        /// <summary>
        /// Set property display name
        /// </summary>
        /// <param name="modelTypeId">model type id</param>
        /// <param name="propertyName">property name</param>
        /// <param name="displayName">display name</param>
        static void SetPropertyDisplay(Guid modelTypeId, string propertyName, string displayName)
        {
            if (modelTypeId.IsEmpty() || string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(displayName))
            {
                return;
            }
            string displayKey = GetDisplayConfigKey(modelTypeId, propertyName);
            if (PropertyDisplays.ContainsKey(displayKey))
            {
                var nowDisplayText = PropertyDisplays[displayKey];
                if (nowDisplayText == null)
                {
                    nowDisplayText = new DisplayText();
                }
                nowDisplayText.DisplayName = displayName;
            }
            else
            {
                PropertyDisplays.Add(displayKey, new DisplayText()
                {
                    DisplayName = displayName
                });
            }
        }

        /// <summary>
        /// Get display config key
        /// </summary>
        /// <param name="modelTypeId">module type id</param>
        /// <param name="propertyName">property name</param>
        /// <returns>return the display config key</returns>
        static string GetDisplayConfigKey(Guid modelTypeId, string propertyName)
        {
            string displayKey = string.Format("{0}_{1}", modelTypeId, propertyName);
            return displayKey;
        }
    }
}
