
/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class WorkflowConfig
{

    private string emailNotificationOnField;

    private WorkflowConfigEmailConfig emailConfigField;

    private WorkflowConfigCMSInfo cMSInfoField;

    private WorkflowConfigWorkflowTaskDefinitions workflowTaskDefinitionsField;

    private WorkflowConfigTask[] eMailMessagesField;

    private WorkflowConfigPublication[] publishMappingsField;

    private string styleClassField;

    /// <remarks/>
    public string EmailNotificationOn
    {
        get
        {
            return this.emailNotificationOnField;
        }
        set
        {
            this.emailNotificationOnField = value;
        }
    }

    /// <remarks/>
    public WorkflowConfigEmailConfig EmailConfig
    {
        get
        {
            return this.emailConfigField;
        }
        set
        {
            this.emailConfigField = value;
        }
    }

    /// <remarks/>
    public WorkflowConfigCMSInfo CMSInfo
    {
        get
        {
            return this.cMSInfoField;
        }
        set
        {
            this.cMSInfoField = value;
        }
    }

    /// <remarks/>
    public WorkflowConfigWorkflowTaskDefinitions WorkflowTaskDefinitions
    {
        get
        {
            return this.workflowTaskDefinitionsField;
        }
        set
        {
            this.workflowTaskDefinitionsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Task", IsNullable = false)]
    public WorkflowConfigTask[] EMailMessages
    {
        get
        {
            return this.eMailMessagesField;
        }
        set
        {
            this.eMailMessagesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Publication", IsNullable = false)]
    public WorkflowConfigPublication[] PublishMappings
    {
        get
        {
            return this.publishMappingsField;
        }
        set
        {
            this.publishMappingsField = value;
        }
    }

    /// <remarks/>
    public string StyleClass
    {
        get
        {
            return this.styleClassField;
        }
        set
        {
            this.styleClassField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigEmailConfig
{

    private string sMTPServerField;

    private byte sMTPPortField;

    private string sMTPUserField;

    private string sMTPPasswordField;

    private string sMTPSSLEnabledField;

    private string sMTPIsBodyHTMLField;

    private string emailFromField;

    /// <remarks/>
    public string SMTPServer
    {
        get
        {
            return this.sMTPServerField;
        }
        set
        {
            this.sMTPServerField = value;
        }
    }

    /// <remarks/>
    public byte SMTPPort
    {
        get
        {
            return this.sMTPPortField;
        }
        set
        {
            this.sMTPPortField = value;
        }
    }

    /// <remarks/>
    public string SMTPUser
    {
        get
        {
            return this.sMTPUserField;
        }
        set
        {
            this.sMTPUserField = value;
        }
    }

    /// <remarks/>
    public string SMTPPassword
    {
        get
        {
            return this.sMTPPasswordField;
        }
        set
        {
            this.sMTPPasswordField = value;
        }
    }

    /// <remarks/>
    public string SMTPSSLEnabled
    {
        get
        {
            return this.sMTPSSLEnabledField;
        }
        set
        {
            this.sMTPSSLEnabledField = value;
        }
    }

    /// <remarks/>
    public string SMTPIsBodyHTML
    {
        get
        {
            return this.sMTPIsBodyHTMLField;
        }
        set
        {
            this.sMTPIsBodyHTMLField = value;
        }
    }

    /// <remarks/>
    public string EmailFrom
    {
        get
        {
            return this.emailFromField;
        }
        set
        {
            this.emailFromField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigCMSInfo
{

    private string previewURLField;

    private string cMSURLField;

    /// <remarks/>
    public string PreviewURL
    {
        get
        {
            return this.previewURLField;
        }
        set
        {
            this.previewURLField = value;
        }
    }

    /// <remarks/>
    public string CMSURL
    {
        get
        {
            return this.cMSURLField;
        }
        set
        {
            this.cMSURLField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigWorkflowTaskDefinitions
{

    private WorkflowConfigWorkflowTaskDefinitionsTask[] emailTasksField;

    private WorkflowConfigWorkflowTaskDefinitionsTarget[] publishTasksField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Task", IsNullable = false)]
    public WorkflowConfigWorkflowTaskDefinitionsTask[] EmailTasks
    {
        get
        {
            return this.emailTasksField;
        }
        set
        {
            this.emailTasksField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Target", IsNullable = false)]
    public WorkflowConfigWorkflowTaskDefinitionsTarget[] PublishTasks
    {
        get
        {
            return this.publishTasksField;
        }
        set
        {
            this.publishTasksField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigWorkflowTaskDefinitionsTask
{

    private string messageIDField;

    private bool toAuthorField;

    private bool toGroupField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string messageID
    {
        get
        {
            return this.messageIDField;
        }
        set
        {
            this.messageIDField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool toAuthor
    {
        get
        {
            return this.toAuthorField;
        }
        set
        {
            this.toAuthorField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool toGroup
    {
        get
        {
            return this.toGroupField;
        }
        set
        {
            this.toGroupField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigWorkflowTaskDefinitionsTarget
{

    private string targetIDField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string targetID
    {
        get
        {
            return this.targetIDField;
        }
        set
        {
            this.targetIDField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigTask
{

    private string subjectField;

    private string bodyField;

    private string messageIDField;

    /// <remarks/>
    public string Subject
    {
        get
        {
            return this.subjectField;
        }
        set
        {
            this.subjectField = value;
        }
    }

    /// <remarks/>
    public string Body
    {
        get
        {
            return this.bodyField;
        }
        set
        {
            this.bodyField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string messageID
    {
        get
        {
            return this.messageIDField;
        }
        set
        {
            this.messageIDField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class WorkflowConfigPublication
{

    private string contentField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Content
    {
        get
        {
            return this.contentField;
        }
        set
        {
            this.contentField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

