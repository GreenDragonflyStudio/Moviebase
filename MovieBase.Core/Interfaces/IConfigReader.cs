namespace Moviebase.Core.Interfaces
{
    public interface IConfigReader
    {
        #region Methods

        ApiSettings GetApiSettings();

        #endregion Methods
    }
}