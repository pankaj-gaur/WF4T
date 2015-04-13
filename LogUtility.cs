using System;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using Tridion.Logging;

namespace Tridion.Extensions.EventSystem
{
    public class LogUtility
    {

       public static ILog GetLogger(string type)
       {
          // If no loggers have been created, load our own.
          if (LogManager.GetCurrentLoggers().Length == 0)
          {
             LoadConfig();
          }
          return LogManager.GetLogger(type);
       }

       private static void LoadConfig()
       {
          //// TODO: Do exception handling for File access issues and supply sane defaults if it's unavailable.   
          XmlConfigurator.ConfigureAndWatch(new FileInfo(WorkflowUtilities.GetTridionInstallPath() + @"\cmsextensions\config\log4net.xml"));
       }     

    }
}
