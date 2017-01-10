//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Modules
{
    public class AttachedModules
    {
        private static List<Type> _allModules;
        private static List<Module> _activeModules;
        private static bool _isLoaded;
        
        public static void LoadModules()
        {
            _activeModules = ModulesRepository.GetModulesFromDb().Where(m => m.IsInstall && m.Active).ToList();

            _allModules =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(
                        item =>
                            item.Namespace == "AdvantShop.Modules" &&
                            item.GetInterface("AdvantShop.Modules.Interfaces.IModule") == typeof (IModule))
                    .ToList();

            _isLoaded = true;
        }

        public static List<Type> GetModules<T>()
        {
            if (!_isLoaded)
                LoadModules();

            var type = typeof(T);

            return _allModules.Where(
                            item =>
                            item.IsClass &&
                            type.IsAssignableFrom(item) &&
                            _activeModules.Any(m => item.Name.ToLower() == m.StringId.ToLower())).ToList();
        }

        public static List<Type> GetModules()
        {
            if (!_isLoaded)
                LoadModules();

            return _allModules;
        }
    }
}