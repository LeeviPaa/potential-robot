namespace PotentialRobot.UI.Style
{
    public interface IStyleComponent
    {
        IStyleReference Reference { get; }
        void Subscribe(IStyleComponent component);
        void Unsubscribe(IStyleComponent component);
        void Apply(IStyle style);
    }
}
