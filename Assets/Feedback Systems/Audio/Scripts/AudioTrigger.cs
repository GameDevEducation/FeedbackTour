using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] SoundEffect Effect;

    public void OnTrigger()
    {
        AudioManager.PlaySoundEffect(Effect, transform.position);
    }
}
