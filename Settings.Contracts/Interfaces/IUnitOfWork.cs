namespace Settings.Contracts.Interfaces;
public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
