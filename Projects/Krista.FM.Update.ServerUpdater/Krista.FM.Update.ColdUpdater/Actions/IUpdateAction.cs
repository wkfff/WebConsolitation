namespace Krista.FM.Update.ColdUpdater.Actions
{
    internal interface IUpdateAction
    {
        bool Do();
        void Rollback(string backupPath);
    }
}
