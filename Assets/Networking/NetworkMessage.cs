public struct NetworkMessage
{
    public readonly string header;
    public readonly object content;
    public readonly PlayerID sender;

    public NetworkMessage(string header, object content, PlayerID sender)
    {
        this.header = header;
        this.content = content;
        this.sender = sender;
    }
}