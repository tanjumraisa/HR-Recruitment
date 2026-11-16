namespace HR.Facade.Interface
{
    public interface IDemoDataFacade
    {
        Task<bool> IsLoadedAsync();
        Task<string> LoadAsync();
    }
}
