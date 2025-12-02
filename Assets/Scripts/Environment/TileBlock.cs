using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class TileBlock : MonoBehaviour
{
    [Header("Tile Settings")]
    public bool isDestructible = true;
    public bool hasGravity = false;
    public int hitPoints = 3;
    private int maxHitPoints;

    [Header("Crack Overlay")]
    public Sprite[] crackSprites;
    private GameObject crackOverlay;
    private SpriteRenderer crackRenderer;

    private SpriteRenderer tileRenderer;
    private bool isFalling = false;
    private bool isWaitingToFall = false;

    private void Awake()
    {
        tileRenderer = GetComponent<SpriteRenderer>();
        maxHitPoints = hitPoints;

        if (crackSprites == null || crackSprites.Length == 0)
        {
            crackSprites = Resources.LoadAll<Sprite>("Cracks");
        }
    }

    private void Update()
    {
        if (hasGravity && !isFalling && !isWaitingToFall)
        {
            CheckForSupportBelow();
        }
    }

    private void CheckForSupportBelow()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground", "Tiles"));
        if (hit.collider == null)
        {
            StartCoroutine(StartFallingDelay());
        }
    }

    private IEnumerator StartFallingDelay()
    {
        isWaitingToFall = true;

        //CameraShake.Instance.Shake(3f, 0.1f);

        yield return new WaitForSeconds(3f);

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        isFalling = true;
        isWaitingToFall = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling) return;

        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(9999);
        }

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Pillar"))
        {
            Destroy(GetComponent<Rigidbody2D>());
            isFalling = false;
        }
    }

    public void TakeDamage(int amount, float destroyDelay = 0f)
    {
        if (!isDestructible) return;

        hitPoints -= amount;
        UpdateCrackSprite();

        if (hitPoints <= 0)
        {
            if (destroyDelay > 0f)
                StartCoroutine(DestroyAfterDelay(destroyDelay));
            else
                DestroyTile();
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyTile();
    }

    private void DestroyTile()
    {
        if (crackOverlay != null)
            Destroy(crackOverlay);
        Destroy(gameObject);
    }

    private void UpdateCrackSprite()
    {
        if (crackSprites == null || crackSprites.Length == 0) return;

        if (hitPoints >= maxHitPoints)
        {
            if (crackOverlay != null)
                crackOverlay.SetActive(false);
            return;
        }

        if (crackOverlay == null)
        {
            crackOverlay = new GameObject("CrackOverlay");
            crackOverlay.transform.SetParent(transform);
            crackOverlay.transform.localPosition = Vector3.zero;

            crackRenderer = crackOverlay.AddComponent<SpriteRenderer>();
            crackRenderer.sortingOrder = tileRenderer.sortingOrder + 1;
        }

        float damagePercent = 1f - ((float)hitPoints / maxHitPoints);
        int crackIndex = Mathf.Clamp(Mathf.CeilToInt(damagePercent * crackSprites.Length) - 1, 0, crackSprites.Length - 1);
        crackRenderer.sprite = crackSprites[crackIndex];
        crackOverlay.SetActive(true);
    }
}
