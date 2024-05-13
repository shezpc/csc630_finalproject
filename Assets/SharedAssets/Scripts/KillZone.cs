using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider2D collision)
    {
        collision.gameObject.SetActive(false); 
    }
}
