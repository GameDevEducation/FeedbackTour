using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleZone : MonoBehaviour
{
    [SerializeField] string TagToCheck = "Player";
    [SerializeField] HapticEffectSO HapticEffect;

    HapticEffectSO ActiveEffect = null;

    List<GameObject> PlayersInZone = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagToCheck))
        {
            // players now in the zone - start the effect
            if (PlayersInZone.Count == 0)
                ActiveEffect = HapticManager.PlayEffect(HapticEffect, transform.position);

            PlayersInZone.Add(other.gameObject);           
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagToCheck))
        {
            PlayersInZone.Remove(other.gameObject);

            // no players left in the zone, stop the effect
            if (PlayersInZone.Count == 0)
            {
                HapticManager.StopEffect(ActiveEffect);
                ActiveEffect = null;
            }
        }
    }
}
