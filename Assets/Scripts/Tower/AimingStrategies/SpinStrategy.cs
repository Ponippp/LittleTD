using UnityEngine;

public class SpinStrategy : IAimingStrategy
{
    private float _currentAngle = 0f;
    private float _rotationSpeed;

    //need a constructor for spin strat but not other strats b/c spin strat needs a var, others dont
    public SpinStrategy(float rotationSpeed)
    {
        _rotationSpeed = rotationSpeed; //same as this.rotationSpeed = rotationSpeed
    }

    public AimingResult UpdateAiming(Vector3 towerPosition, float range)
    {
        //update curr angle by adding speed*time, mod 360 to keep it between 0 and 360
        _currentAngle = (_currentAngle + _rotationSpeed * Time.deltaTime) % 360f;

        //can also initialize this in struct syntax like {a,b,c} instead of setting each variable one by one
        AimingResult result = new AimingResult();
        result.enemy = null;
        result.shouldFire = true;
        result.lookingAngle = _currentAngle;

        // Firing angle is 10 degrees in front of the tower's current angle so it doesnt look weird in game
        Vector3 direction = new Vector3(Mathf.Cos(_currentAngle * Mathf.Deg2Rad), Mathf.Sin(_currentAngle * Mathf.Deg2Rad), 0f);
        result.targetPosition = towerPosition + direction * 10f;

        return result;
    }
}
