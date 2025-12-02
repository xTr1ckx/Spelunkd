using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class TileBlockNew : MonoBehaviour
{
    [Header("Tile Settings")]
    public bool isDestructible = true;
    public bool hasGravity = false;
    public int hitPoints = 3;

    [Header("Crack Overlay")]
    public Sprite[] crackSprites;
    private GameObject crackOverlay;
    private SpriteRenderer crackRenderer;

    private SpriteRenderer sr;

    [HideInInspector] public GridManagerNew gridManager;
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;

    private bool isFalling = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (crackSprites == null || crackSprites.Length == 0)
        {
            crackSprites = Resources.LoadAll<Sprite>("Cracks");
        }
    }

    private void Update()
    {
        if (hasGravity && !isFalling)
        {
            if (!gridManager.IsSupported(gridX, gridY))
            {
                // Tell GridManager: "I should fall now"
                gridManager.MakeTileFall(gridX, gridY);
                isFalling = true;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isDestructible) return;

        hitPoints -= amount;
        UpdateCrackSprite();

        if (hitPoints <= 0)
        {
            DestroyTile();
        }
    }

    private void DestroyTile()
    {
        gridManager.RemoveTileAt(gridX, gridY);

        if (crackOverlay != null)
            Destroy(crackOverlay);

        Destroy(gameObject);
    }

    private void UpdateCrackSprite()
    {
        if (crackSprites == null || crackSprites.Length == 0) return;

        if (hitPoints >= 3)
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
            crackRenderer.sortingOrder = sr.sortingOrder + 1;
        }

        float damagePercent = 1f - ((float)hitPoints / 3f);
        int crackIndex = Mathf.Clamp(Mathf.FloorToInt(damagePercent * crackSprites.Length), 0, crackSprites.Length - 1);

        crackRenderer.sprite = crackSprites[crackIndex];
        crackOverlay.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling) return;

        PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(9999);
        }

        // Once a falling tile hits something, stop falling.
        if (collision.collider.CompareTag("Ground") ||
            collision.collider.CompareTag("Tile") ||
            collision.collider.CompareTag("Pillar"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) Destroy(rb);

            isFalling = false;
        }
    }
}

