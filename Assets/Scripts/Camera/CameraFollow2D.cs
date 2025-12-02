using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public static CameraFollow2D Instance;

    [Tooltip("The transform (usually the Player) that the camera will follow")]
    public Transform target;

    [Tooltip("Offset from the target's position")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Tooltip("Approximate time to catch up to the target (0 = instant, higher = slower)")]
    public float smoothTime = 0.2f;

    [Range(0f, 1f), Tooltip("How much the camera moves toward the mouse (0 = no look-ahead, 1 = camera centers on mouse)")]
    public float mouseInfluence = 0.3f;

    private Vector3 velocity;

    [Header("Shake Settings")]
    private Coroutine shakeCoroutine;
    private Vector3 shakeOffset = Vector3.zero;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -offset.z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        
        Vector3 playerCamPos = target.position + offset;
        Vector3 blendedTarget = Vector3.Lerp(playerCamPos, new Vector3(mouseWorld.x, mouseWorld.y, playerCamPos.z), mouseInfluence);
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, blendedTarget, ref velocity, smoothTime);

        transform.position = smoothPos + shakeOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}
