namespace Ame.Modules.Docks
{
    public interface ILayoutViewModel
    {
        bool IsBusy { get; set; }
        string AppDataDirectory { get; }
    }
}
