using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [SerializeField] private HeroData _heroData;

    [SerializeField] private AudioSource _audio;

    private void OnEnable()
    {
        SetSpawnSound();
    }

    private void SetSpawnSound()
    {
        if(PlayerController.Instance.PlayerData.IsSound == true)
        {
            if(_heroData.SpawnClip != null)
            {
                _audio.PlayOneShot(_heroData.SpawnClip);
            }
        }
    }
}
