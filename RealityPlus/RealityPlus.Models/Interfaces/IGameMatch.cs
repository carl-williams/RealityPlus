namespace RealityPlus.Models.Interfaces
{
    public interface IGameMatch
    {
        IEnumerable<string> Quick(Guid sessonId);

        IEnumerable<string> Ranked(Guid sessonId);
    }
}
