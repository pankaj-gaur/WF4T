using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Extensibility;
using Tridion.ContentManager.Extensibility.Events;
using Tridion.ContentManager.Publishing;
using Tridion.ContentManager.Security;
using Tridion.ContentManager.Workflow;
using log4net;

namespace Tridion.Extensions.EventSystem
{
    [TcmExtension("WorkflowNotificationAndAutomation")]
    public class WorkflowAutomation : TcmExtension
    {
        private static readonly ILog log = LogUtility.GetLogger("WorkflowAutomation");
        private WorkflowConfig configInstance = null;

        public WorkflowAutomation()
        {
            log.Info("Entering Workflow Automation Constructor...");
            Subscribe();
            log.Info("Exiting Workflow Automation Constructor...");
        }

        private void Subscribe()
        {
            log.Info("Subscribing to Workflow notification event system");
            EventSystem.Subscribe<ActivityInstance, FinishActivityEventArgs>(WorkflowNotifyandPublish, EventPhases.Initiated);
            log.Info("Subscribed to Workflow notification event system successfully");
        }

        private void WorkflowNotifyandPublish(ActivityInstance subject, FinishActivityEventArgs args, EventPhases phase)
        {
            try
            {
                string currentActivityTitle = subject.ActivityDefinition.Title;
                currentActivityTitle = currentActivityTitle.ToUpper();
                log.Info("============== Current Activity Title: " + currentActivityTitle + "==============");

                XmlDocument configXML = XMLConfigReader.GetXmlConfig();
                if (configXML != null && !string.IsNullOrEmpty(configXML.InnerXml))
                {
                    log.Info("Loading Configuration... Successful");
                    configInstance = Deserialize<WorkflowConfig>(configXML);
                    log.Info("Deserializing Configuration... Successful");

                    IEnumerable<WorkItem> workItems = subject.WorkItems;
                    WorkItem curWorkItem;
                    using (IEnumerator<WorkItem> iter = workItems.GetEnumerator())
                    {
                        iter.MoveNext();
                        curWorkItem = iter.Current;
                    }

                    log.Info("Current Work Item... " + curWorkItem != null ? "Not Null" : "Null");

                    var publishInfo = from p in configInstance.WorkflowTaskDefinitions.PublishTasks
                                      where p.Value.Equals(currentActivityTitle)
                                      select p;

                    if (publishInfo != null && publishInfo.Count() > 0)
                    {
                        log.Info("Publishing Process... Starting");
                        PublishItem(publishInfo, curWorkItem, subject.Session);
                        log.Info("Publishing Process... Finished");
                    }
                    else
                    {
                        log.Info("No Publish Info...");
                    }


                    var sendEmailInfo = from e in configInstance.WorkflowTaskDefinitions.EmailTasks
                                        where e.Value.Equals(currentActivityTitle)
                                        select e;

                    if (sendEmailInfo != null && sendEmailInfo.Count() > 0 && configInstance.EmailNotificationOn.ToUpper() == "TRUE")
                    {
                        log.Info("Email Notification Process... Starting");
                        SendEmailNotification(sendEmailInfo, subject, curWorkItem, args.ActivityFinish.Message);
                        log.Info("Email Notification Process... Finished");
                    }
                    else
                    {
                        log.Info("No Email Notification Info");
                    }
                }
                else
                {
                    log.Warn("No Configuration XML or Error in Loading Configuration File");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Finishing Workflow Activity. ERROR: " + ex.Message + ex.StackTrace);
            }
            finally
            {
                log.Info("============== Finished Workflow Activity ==============");
            }
        }

        private void PublishItem(IEnumerable<WorkflowConfigWorkflowTaskDefinitionsTarget> publishInfo, WorkItem curWorkItem, Session session)
        {
            if (curWorkItem != null)
            {
                try
                {
                    TcmUri workItemUri = new TcmUri(curWorkItem.Subject.Id);
                    log.Info("Work Item Item ID: " + workItemUri.ItemId);
                    log.Info("Work Item Publication ID: " + workItemUri.ContextRepositoryId);

                    List<IdentifiableObject> items = new List<IdentifiableObject>();
                    List<TargetType> targets = new List<TargetType>();
                    foreach (WorkflowConfigWorkflowTaskDefinitionsTarget t in publishInfo)
                    {
                        TcmUri targetURI = new TcmUri(t.targetID);
                        targets.Add(new TargetType(targetURI, session));
                        log.Info("Adding Target... Successful. Target ID: " + t.targetID);
                    }
                    log.Info("Target Count: " + targets.Count().ToString());

                    if (workItemUri.ItemType == ItemType.Component)
                    {
                        log.Info("Checking Item Type... Component");
                        Component curComp = (Component)curWorkItem.Session.GetObject(curWorkItem.Subject.Id);
                        var publishFrom = from p in configInstance.PublishMappings
                                          where p.Content.Equals(curComp.ContextRepository.Title)
                                          select p;

                        string PublishFrom = publishFrom != null && publishFrom.Count() > 0 ? publishFrom.FirstOrDefault().Value : string.Empty;
                        log.Debug("Workflow Item original publication : " + curComp.ContextRepository.Title + Environment.NewLine + " Publishable publication : " + PublishFrom);

                        if (!String.IsNullOrEmpty(PublishFrom))
                        {
                            Repository localRepository = (Repository)curWorkItem.Session.GetObject(PublishFrom);
                            TcmUri websiteTcmUri = new TcmUri(localRepository.Id);
                            log.Debug("Web Site TCM URI: " + localRepository.Id);
                            TcmUri curCompTcmUri = curComp.Id;
                            TcmUri localCompTcmUri = new TcmUri(curCompTcmUri.ItemId, curCompTcmUri.ItemType, websiteTcmUri.ContextRepositoryId);
                            log.Info("Workflow Item URIs --> Original : " + curCompTcmUri + " Local : " + localCompTcmUri);
                            Component localComponent = new Component(localCompTcmUri, curWorkItem.Session);
                            items.Add(localComponent);
                        }
                        else
                        {
                            items.Add(curComp);
                        }
                    }
                    else
                    {
                        Page curpage = (Page)curWorkItem.Session.GetObject(curWorkItem.Subject.Id);
                        log.Info("Checking Item Type... Page. Page URI : " + curpage.Id);
                        items.Add(curpage);
                    }

                    log.Info("Constructing Resolve Instructions...");
                    ResolveInstruction resolveInstruction = new ResolveInstruction(session);
                    resolveInstruction.IncludeWorkflow = true;
                    resolveInstruction.Purpose = ResolvePurpose.Publish;
                    PublishInstruction publishInstruction = new PublishInstruction(session);
                    publishInstruction.ResolveInstruction = resolveInstruction;
                    log.Info("Resolve Instructions constructed");
                    log.Info("Sending Workflow Item to Publishing Queue...");
                    PublishEngine.Publish(items, publishInstruction, targets, PublishPriority.High);
                    log.Info("Workflow Item sent to Publishing Queue with high Priroty");

                }
                catch (Exception ex)
                {
                    log.Fatal("Error Publishing item from Workflow --> " + ex.Message + ex.InnerException.Message);
                }
            }
            else
            {
                log.Info("No Current Workflow Item Information");
            }
            log.Info("Publish Process Finished");
        }

        private void SendEmailNotification(IEnumerable<WorkflowConfigWorkflowTaskDefinitionsTask> sendEmailInfo, ActivityInstance subject, WorkItem curWorkItem, string activityMessage)
        {
            try
            {
                var emailInfo = sendEmailInfo.FirstOrDefault();
                //Get Activity Owner Information
                log.Info("Getting Performer Info....");
                User perfomer = GetPerformerInformation(subject);
                log.Info("Performer: "+perfomer != null ? perfomer.Description : "NULL");
                //Get List of Recepients

                List<string> receipients = GetEmailRecepients(emailInfo, subject, perfomer);
                
                // To Email Address
                string toEmail = String.Join(";", receipients);
                log.Info("Receipients: " + toEmail);

                // From Email
                string fromEmail = configInstance.EmailConfig.EmailFrom;
                log.Info("From: " + fromEmail);

                // SMTP Server Detail
                string smtpServer = configInstance.EmailConfig.SMTPServer;
                log.Info("SMTP Server: " + smtpServer);

                var emailMessage = GetEmailMessageInformation(curWorkItem, perfomer, activityMessage, emailInfo.messageID);

                // Email Subject
                string emailSubject = emailMessage.Subject;
                log.Info("Email Subject: " + emailSubject);

                //Email Body
                string emailBody = emailMessage.Body;
                log.Info("Email Body: " + emailMessage.Body);
                
                WorkflowUtilities.sendWFNotification(fromEmail, toEmail, emailSubject, emailBody, smtpServer, Convert.ToBoolean(configInstance.EmailConfig.SMTPIsBodyHTML));
            }
            catch (Exception ex)
            {
                log.Error("Error in Sending Email Notification. Error: " + ex.Message + ex.StackTrace);
            }
        }

        private WorkflowConfigTask GetEmailMessageInformation(WorkItem curWorkItem, User perfomer, string activityMessage, string messageID)
        {
            string itemCmsPath = string.Empty;
            string itemTitle = string.Empty;
            string itemType = string.Empty;
            string itemPubName = string.Empty;
            string itemTcmType = string.Empty;
            string itemID = string.Empty;

            TcmUri workItemUri = new TcmUri(curWorkItem.Subject.Id);
            if (workItemUri.ItemType == ItemType.Component)
            {
                Component curComp = (Component)curWorkItem.Session.GetObject(curWorkItem.Subject.Id);
                itemCmsPath = curComp.Path;
                itemTitle = curComp.Title;
                itemType = curComp.Schema.Title;
                itemPubName = curComp.ContextRepository.Title + " (" + curComp.ContextRepository.Id + ")";
                itemTcmType = ((int)ItemType.Component).ToString();
                itemID = curComp.Id;
            }
            else
            {
                Page curpage = (Page)curWorkItem.Session.GetObject(curWorkItem.Subject.Id);
                itemCmsPath = curpage.Path.Replace("\\" + curpage.ContextRepository.Title, "").ToString();
                itemTitle = curpage.Title;
                itemType = "Page";
                itemPubName = curpage.ContextRepository.Title;
                itemTcmType = ((int)ItemType.Page).ToString();
                itemID = curpage.Id;
            }
            var emailMessageInfo = from sub in configInstance.EMailMessages
                                   where sub.messageID == messageID
                                   select sub;
            WorkflowConfigTask emailMessage = emailMessageInfo != null && emailMessageInfo.Count() > 0 ? emailMessageInfo.FirstOrDefault() : null;
            if (emailMessage != null)
            {
                string strSubject = emailMessage.Subject;
                string strEmailBody = emailMessage.Body;

                if (!string.IsNullOrEmpty(strSubject))
                {
                    strSubject = string.Format(strSubject, itemTitle);
                    emailMessage.Subject = strSubject;

                }
                if (!string.IsNullOrEmpty(strEmailBody))
                {
                    strEmailBody = string.Format(strEmailBody, itemTitle, itemType, itemCmsPath, perfomer.Title, perfomer.Description,
                                                    activityMessage, itemPubName, configInstance.CMSInfo.CMSURL, itemTcmType,
                                                    itemID, configInstance.CMSInfo.PreviewURL, configInstance.StyleClass);
                    emailMessage.Body = strEmailBody;
                }
            }
            return emailMessage;
        }

        private List<string> GetEmailRecepients(WorkflowConfigWorkflowTaskDefinitionsTask emailInfo, ActivityInstance subject, User performer)
        {
            List<string> recepients = new List<string>();
            if (emailInfo != null)
            {
                if (emailInfo.toGroup)
                {
                    recepients = GetGroupAssigneeInformation(subject);
                }
                if (emailInfo.toAuthor && performer != null && performer.Description != null)
                {
                    string performerEmail = GetEmailAddressFromFullName(performer.Description);
                    if (!string.IsNullOrEmpty(performerEmail))
                    {
                        recepients.Add(performerEmail);
                    }
                }
            }
            return recepients;
        }

        private List<string> GetGroupAssigneeInformation(ActivityInstance subject)
        {
            log.Info("Get Group Assignee Information");
            List<string> approvalUserList = new List<string>();
            if (subject != null)
            {
                ActivityDefinition approvalDefinition = GetWorkflowActivityAtItem(subject, 2);
                if (approvalDefinition != null)
                {
                    log.Info("Approval Activity Title " + approvalDefinition.Title);
                    log.Info("Approval Activity Assignee " + approvalDefinition.Assignee);
                    log.Info("Approval Activity Description " + approvalDefinition.Description);
                    approvalUserList = GetAssigneesForWorkflowStep(approvalDefinition, approvalUserList);
                    approvalUserList = approvalUserList.Distinct().ToList();
                    log.Info("Group Assignee Information Got successfully");
                }
            }
            return approvalUserList;
        }

        private User GetPerformerInformation(ActivityInstance subject)
        {
            log.Info("Getting Performer Information...");
            User performer = null;
            if (subject != null && subject.Process != null)
            {
                IEnumerable<Activity> curWorkActivities = subject.Process.Activities;
                if (curWorkActivities != null && curWorkActivities.Count() > (subject.Position - 2))
                {
                    Activity curActivity = (Activity)curWorkActivities.ToArray<Activity>().GetValue(subject.Position - 2);

                    // get the User who inititaed the workflow
                    if (curActivity != null && curActivity.Performers != null && curActivity.Performers.Count() > 0)
                    {
                        using (IEnumerator<User> iter = curActivity.Performers.GetEnumerator())
                        {
                            iter.MoveNext();
                            performer = iter.Current;
                        }
                    }
                    else
                    {
                        log.Info("curActivity is NULL or curActivity.Performers does not have records");
                    }
                }
                else
                {
                    log.Info("curWorkActivities is NULL or curWorkActivities.Count < (subject.Position - 2)");
                }
            }
            else
            {
                log.Info("subject or subject.Process is NULL");
            }
            return performer;
        }

        private T Deserialize<T>(XmlDocument xmlDocument)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            
            StringReader reader = new StringReader(xmlDocument.InnerXml);
            XmlReader xmlReader = new XmlTextReader(reader);
            return (T)ser.Deserialize(xmlReader);
        }

        private static ActivityDefinition GetWorkflowActivityAtItem(ActivityInstance subject, int index)
        {
            IEnumerable<ActivityDefinition> activityDefinition = subject.ActivityDefinition.ProcessDefinition.ActivityDefinitions;
            return (ActivityDefinition)activityDefinition.ToArray<ActivityDefinition>().GetValue(index);
        }

        private static List<String> GetUserEmailAddress(Group grpName, List<String> userEmailAddress)
        {
            IEnumerable<Trustee> grpMembers = grpName.GetGroupMembers();
            IEnumerator<Trustee> enumGrpMembers = grpMembers.GetEnumerator();
            while (enumGrpMembers.MoveNext())
            {
                TcmUri memberType = new TcmUri(enumGrpMembers.Current.Id);
                if (memberType.ItemType == ItemType.User)
                {
                    string userEmail = string.Empty;
                    // description as Full Name (email Address)
                    log.Info("User Full Name : " + enumGrpMembers.Current.Description);
                    userEmail = GetEmailAddressFromFullName(enumGrpMembers.Current.Description);
                    log.Info("User Email Address : " + userEmail);
                    userEmailAddress.Add(userEmail);
                }
                else
                {
                    log.Debug("Workflow assigned contains Group : " + enumGrpMembers.Current.Description);
                    GetUserEmailAddress((Group)enumGrpMembers.Current, userEmailAddress);
                }
            }
            return userEmailAddress;
        }

        private static List<String> GetAssigneesForWorkflowStep(ActivityDefinition approvalDefinition, List<String> approvalUserList)
        {
            Trustee approvalTrustee = (Trustee)approvalDefinition.Assignee;
            TcmUri trusteeType = new TcmUri(approvalTrustee.Id);
            if (trusteeType.ItemType == ItemType.Group)
            {
                Group approvalGroup = (Group)approvalTrustee;
                if (!approvalGroup.IsPredefined)
                {
                    approvalUserList = GetUserEmailAddress(approvalGroup, approvalUserList);
                }
            }
            return approvalUserList;
        }

        private static String GetEmailAddressFromFullName(string userDescription)
        {
            // Email Address Format USER FULLNAME (EMAILADDRESS), extract EMAILADDRESS
            string strRegex = @"(.*?) \((.*?)\)";
            System.Text.RegularExpressions.RegexOptions usrRegExOptions = System.Text.RegularExpressions.RegexOptions.None;
            System.Text.RegularExpressions.Regex usrRegex = new System.Text.RegularExpressions.Regex(strRegex, usrRegExOptions);
            string strReplace = "$2";
            return usrRegex.Replace(userDescription, strReplace);
        }

    }
}
