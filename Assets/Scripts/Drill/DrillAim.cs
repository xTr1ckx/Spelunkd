using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillAim : MonoBehaviour
{
    public Transform drillSprite;
    //public Transform projectileSpawnPoint;
    public Transform player;
    public SpriteRenderer drillRenderer;

    //private float originalSpawnX = 1.094f;
    //private float flippedSpawnX = -1.094f;

    //private float originalSpawnY = -0.206f;
    //private float flippedSpawnY = 0.206f;

    private void Update()
    {
        RotateTowardMouse();
        SyncFlipWithPlayer();
    }

    void RotateTowardMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void SyncFlipWithPlayer()
    {
        if (player.localScale.x < 0)
        {
            drillSprite.localScale = new Vector3(-1, -1, 1);
            drillRenderer.sortingOrder = 6;
            //projectileSpawnPoint.localPosition = new Vector3(flippedSpawnX, flippedSpawnY, projectileSpawnPoint.localPosition.z);
        }
        else
        {
            drillSprite.localScale = new Vector3(1, 1, 1);
            drillRenderer.sortingOrder = -1;
            //projectileSpawnPoint.localPosition = new Vector3(originalSpawnX, originalSpawnY, projectileSpawnPoint.localPosition.z);
        }
    }
}
