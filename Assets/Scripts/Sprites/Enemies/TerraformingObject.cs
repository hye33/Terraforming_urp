using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerraformingObject : MonoBehaviour
{
    private PlayerController _player;

    Vector2 zeroVelo = Vector2.zero;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        StartCoroutine(TrackPlayer());
    }

    private IEnumerator TrackPlayer()
    {
        while (true)
        {
            //transform.position = Vector2.SmoothDamp(transform.position, _player.transform.position, ref zeroVelo, 0.5f);
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, 0.08f);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.GetTerraformingGauge(1);
            Destroy(gameObject);
        }
    }
}
