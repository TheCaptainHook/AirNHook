using UnityEngine;

public interface IInhalable
{
    public void Inhalation(Transform accessor);
    public void StopInhale(GameObject accessor);
    public void Fixed(bool value);
    public bool Inhaling(bool value, GameObject player);
    public void Shooting(Vector2 force);
    public bool CanInhale();
}
