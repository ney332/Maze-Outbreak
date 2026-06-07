using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;
    public KeyCode toggleKey = KeyCode.F;

    private void Awake()
    {
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
}
