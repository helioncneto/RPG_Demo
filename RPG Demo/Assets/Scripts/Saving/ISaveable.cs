namespace RPG.Saving
{
    public interface ISaveable
    {
        object GetStates();
        void RestoreState(object state);
    }
}