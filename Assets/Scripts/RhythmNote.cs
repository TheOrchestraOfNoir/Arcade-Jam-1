using UnityEngine;
using TMPro;

/// <summary>
/// A falling arrow shown during rhythm duels. Visual feedback only — scoring still uses BeatConductor timing.
/// </summary>
public class RhythmNote : MonoBehaviour
{
    public TextMeshProUGUI arrowText;
    public float fallSpeed = 400f;
    public float destroyBelowY = -300f;

    public void Setup(string arrow, float speed)
    {
        if (arrowText != null) arrowText.text = arrow;
        fallSpeed = speed;
    }

    private void Update()
    {
        transform.localPosition += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.localPosition.y < destroyBelowY)
        {
            Destroy(gameObject);
        }
    }
}
