namespace _Main.Scripts.Multiplayer.Multiplayer.Obstacles.Interfaces
{
    /// <summary>
    /// Hasar alabilecek objelerin implemente edeceği interface.
    /// PlayerHealth gibi classlara ekle.
    /// </summary>
    public interface IObstacleDamageable
    {
        void TakeDamage(float amount);
    }
}