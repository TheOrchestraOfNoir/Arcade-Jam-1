using UnityEngine;

/// <summary>
/// Temporary buffs on a player (speed, faster shooting).
/// </summary>
public class PlayerBuffs : MonoBehaviour
{
    public float speedMultiplier = 1f;
    public float shootIntervalMultiplier = 1f;

    private float _speedBuffTimer;
    private float _shootBuffTimer;

    private void Update()
    {
        if (_speedBuffTimer > 0f)
        {
            _speedBuffTimer -= Time.deltaTime;
            if (_speedBuffTimer <= 0f) speedMultiplier = 1f;
        }

        if (_shootBuffTimer > 0f)
        {
            _shootBuffTimer -= Time.deltaTime;
            if (_shootBuffTimer <= 0f) shootIntervalMultiplier = 1f;
        }
    }

    public void ApplySpeedBuff(float duration, float multiplier)
    {
        speedMultiplier = multiplier;
        _speedBuffTimer = duration;
    }

    public void ApplyShootBuff(float duration, float multiplier)
    {
        shootIntervalMultiplier = multiplier;
        _shootBuffTimer = duration;
    }
}
