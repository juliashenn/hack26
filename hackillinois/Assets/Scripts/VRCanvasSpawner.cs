using UnityEngine;

public class VRCanvasSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead; // usually OVR CameraRig / CenterEyeAnchor

    [Header("Spawn Settings")]
    public Vector3 localOffset = new Vector3(0, -0.1f, 1.2f); // relative to head
    public Vector3 localEuler = Vector3.zero;                  // rotation relative to head

    private Vector3 defaultScale;

    void Awake()
    {
        defaultScale = transform.localScale;
        gameObject.SetActive(false); // optional start hidden
    }

    void OnEnable()
    {
        // Reset position and rotation relative to head
        transform.position = playerHead.position
                             + playerHead.forward * localOffset.z
                             + playerHead.up * localOffset.y
                             + playerHead.right * localOffset.x;

        transform.rotation = Quaternion.Euler(localEuler) * Quaternion.LookRotation(transform.position - playerHead.position);

        // Reset scale in case it was changed while grabbed
        transform.localScale = defaultScale;
    }
}