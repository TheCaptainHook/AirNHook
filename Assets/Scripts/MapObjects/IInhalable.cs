using UnityEngine;

public interface IInhalable
{
    public void Inhalation(Transform accesor);
    public void StopInhale();
    public void Fixed(bool value);
    public void Inhaling(bool value);
    public void Shooting(Vector2 force);
    public bool CanInhale();
}
