namespace deskpi
{
    public interface IDeskPiMode
    {
        IDeskPiMode ReceiveKey(Key key);
        string Text { get; }
    }
}
