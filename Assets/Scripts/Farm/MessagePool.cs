using System.Collections.Generic;

public class MessagePool : SingleDataMgr<MessagePool>
{
    private Dictionary<int, Queue<string>> messageDict = new Dictionary<int, Queue<string>>();
    
    public void SendMessageTo(int targetId, string message)
    {
        if (!messageDict.ContainsKey(targetId))
        {
            messageDict[targetId] = new Queue<string>();
        }
        messageDict[targetId].Enqueue(message);
    }

    public List<string> RetrieveMessages(int receiverId)
    {
        if (!messageDict.ContainsKey(receiverId))
            return new List<string>();

        var messages = new List<string>(messageDict[receiverId]);
        messageDict[receiverId].Clear();
        return messages;
    }

    public bool HasMessages(int receiverId)
    {
        return messageDict.ContainsKey(receiverId) && messageDict[receiverId].Count > 0;
    }
}

public partial class MessageExecutor
{
    public void ExecuteMessage(string mes)
    {
        
    }
}