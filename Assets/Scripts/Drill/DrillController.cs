using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillController : MonoBehaviour
{
    [Header("Current Drill Stats")]
    public DrillStats currentDrill;

    [Header("Settings")]
    public LayerMask tileLayer;

    [Header("References")]
    public Animator drillAnimator;

    private float nextDrillTime = 0f;
    private bool isDrillingAnimationPlaying = false;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryDrill();
        }
    }

    void TryDrill()
    {
        if (Time.time < nextDrillTime) return;

        nextDrillTime = Time.time + currentDrill.drillCooldown;

        if (!isDrillingAnimationPlaying)
            StartCoroutine(PlayDrillAnimation());

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, currentDrill.drillRange, tileLayer);
        if (hit.collider != null && hit.collider.CompareTag("Tile"))
        {
            TileBlock tile = hit.collider.GetComponent<TileBlock>();
            if(tile != null)
            {
                tile.TakeDamage(currentDrill.drillDamage, currentDrill.drillCooldown);
            }
        }

        Debug.DrawRay(transform.position, transform.right * currentDrill.drillRange, Color.red, 0.1f);
    }

    private IEnumerator PlayDrillAnimation()
    {
        isDrillingAnimationPlaying = true;
        drillAnimator.SetBool("isDrilling", true);

        yield return new WaitForSeconds(currentDrill.drillCooldown);

        drillAnimator.SetBool("isDrilling", false);
        isDrillingAnimationPlaying = false;
    }

    public void EquipDrill(DrillStats newDrill)
    {
        currentDrill = newDrill;
        Debug.Log("Equipped drill: " + newDrill.drillName);
    }
}
