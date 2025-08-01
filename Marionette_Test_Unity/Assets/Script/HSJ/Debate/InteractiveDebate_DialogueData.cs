using SimpleJSON;
using UnityEngine;

public class InteractiveDebate_DialogueData : DialogueData
{
    public string targetName;

    public InteractiveDebate_DialogueData(JSONNode node) : base(node)
    {

    }

    public InteractiveDebate_DialogueData(string[] row) : base(row)
    {

    }
}
