using UnityEngine;

public interface IInhalable
{
    public void Inhalation(Transform accessor);
    public void StopInhale();
    public void Inhaling(bool value);
    public void Shooting(Vector2 force);
    public bool CanInhale();
}
