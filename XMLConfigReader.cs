using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Tridion.Extensions.EventSystem;
using log4net;



namespace Tridion.Extensions.EventSystem
{
   public class XMLConfigReader
   {
      private static DateTime configLastModified = DateTime.Now;
      private static XmlDocument configDoc;
      private static readonly ILog log = LogUtility.GetLogger("WorflowAutomation");
      private static string filePath = WorkflowUtilities.GetTridionInstallPath() + @"\cmsextensions\config\workflow_config.xml";

      public static XmlDocument GetXmlConfig()
      {
         FileInfo configFileInfo = new FileInfo(filePath);
         if (configFileInfo.LastWriteTime.CompareTo(configLastModified) > 0)
         {
            log.Debug("Workflow Config File : " + filePath);
            log.Debug("Workflow Config File Reloading from the LastModified date ");
            configLastModified = configFileInfo.LastWriteTime;
            log.Debug("Workflow Config File LastModified: " + configLastModified);
            configDoc = new XmlDocument();
            configDoc.Load(filePath);
         }
         // First time need to be handled so load always
         if (configDoc == null)
         {
            log.Debug("Workflow Config File : " + filePath);
            configLastModified = configFileInfo.LastWriteTime;
            configDoc = new XmlDocument();
            configDoc.Load(filePath);
            log.Debug("Workflow Config File Loaded for the first time use");
         }
         return configDoc;
      }
      public static string GetConfigValue(string valueToFind)
		{
			string output = string.Empty;
			try
			{
            XmlNode node = GetXmlConfig().SelectSingleNode("/WorkflowConfig/" + valueToFind);
				output = node.InnerText;
            log.Debug("Workflow Config Value For : " + valueToFind + " : " + output);
			}
			catch (Exception ex)
			{
            log.Fatal("Error rconfiguration file. \n" + ex.Message);
			}
			return output;
		}

      public static string GetConfigMapValue(string valueToFind)
      {
         string output = string.Empty;
         try
         {
            XmlNode node = GetXmlConfig().SelectSingleNode("/WorkflowConfig/" + valueToFind);
            Type XmlNodeType = typeof(XmlNode);
            if (XmlNodeType.IsInstanceOfType(node))
            {
               output = node.InnerText;
               log.Debug("Workflow Config Value For : " + valueToFind + " : " + output);
            }
            else
            {
               output = string.Empty;
            }
         }
         catch (Exception ex)
         {
            log.Fatal("Error rconfiguration file. \n" + ex.Message);
         }

         return output;

      }

    

   }
}
