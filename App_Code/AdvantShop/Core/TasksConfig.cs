//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace AdvantShop.Core
{
    public class TasksConfig : IConfigurationSectionHandler
    {
        //public static TasksConfig Instance;
        //private static TasksConfig _instance = null;
        private static List<XmlNode> _scheduleTasks;

        public object Create(object parent, object configContext, XmlNode section)
        {
            //_scheduleTasks = section.SelectSingleNode("Tasks");
            //return null;
            _scheduleTasks = new List<XmlNode>();
            foreach (XmlNode child in section.ChildNodes)
            {
                _scheduleTasks.Add(child);
            }
            return (_scheduleTasks);
        }
    }
}