
public interface IMovementStrategy
{
    void Move();
    float GetDistanceToGoal();
    void Cleanup();
}
