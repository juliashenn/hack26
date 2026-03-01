using System.Collections.Generic;

[System.Serializable]
public class FormQuestion
{
    public string questionText;
    public string commentText;
    // Callbacks set at runtime if needed
}

[System.Serializable]
public class FormSection
{
    public string sectionHeader;
    public List<FormQuestion> questions = new List<FormQuestion>();
}

[System.Serializable]
public class FormData
{
    public string formTitle;
    public List<FormSection> sections = new List<FormSection>();
}
