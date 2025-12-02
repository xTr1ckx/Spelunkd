using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int gridPos;
    //private float checkInterval = 0.5f;
    //public GameObject topPlatform;  //platform
    //public float checkDelay = 0.05f;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        gridPos = gridManager.GetClosestGridPosition(transform.position);
        //InvokeRepeating(nameof(CheckSupport), checkInterval, checkInterval);
        //Invoke(nameof(UpdateTopPlatformState), checkDelay); //platform
    }

    //private void UpdateTopPlatformState() //platform
    //{
    //    Vector2Int abovePos = new Vector2Int(gridPos.x, gridPos.y + 1);
    //    bool ladderAbove = gridManager.IsLadderAt(abovePos);
    //    topPlatform.SetActive(ladderAbove);
    //}

    //private void CheckSupport()    //support, maybe can be used for when the ladder blows up
    //{
    //    Vector2Int belowPos = new Vector2Int(gridPos.x, gridPos.y - 1);
    //    bool supported = false;

    //    if(belowPos.y >= 0)
    //    {
    //        if (gridManager.IsTileAt(belowPos)) supported = true;
    //        else if (gridManager.IsLadderAt(belowPos)) supported = true;
    //    }

    //    if (!supported)
    //    {
    //        gridManager.RemoveStructureAt(gridPos);
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    player.EnterLadder(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
                player.ExitLadder(this);

        }
    }

    //public void RefreshPlatformState()   //platform
    //{
    //    UpdateTopPlatformState();
    //}
}
