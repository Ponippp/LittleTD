using UnityEngine;

public class SpinStrategy : IAimingStrategy
{
    private float _currentAngle = 0f;
    private float _rotationSpeed;

    public SpinStrategy(float rotationSpeed)
    {
        _rotationSpeed = rotationSpeed;
    }

    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        _currentAngle = (_currentAngle + _rotationSpeed * Time.deltaTime) % 360f;

        AimingResult result = new AimingResult();
        result.enemy = null;
        result.shouldFire = true;
        result.lookingAngle = _currentAngle;

        // Firing position is 10 units in front of the tower's current angle
        Vector3 direction = new Vector3(Mathf.Cos(_currentAngle * Mathf.Deg2Rad), Mathf.Sin(_currentAngle * Mathf.Deg2Rad), 0f);
        result.targetPosition = towerPosition + direction * 10f;

        return result;
    }
}
