using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    GameObject _bullet;
    public Transform _aim;
    private Vector3 _delta = new Vector3(0.5f, 1.2f, 0);

    // audio 볼륨 조절 추가해야 함
    private AudioSource _audio;

    SpriteRenderer _aimSprite;
    Color32 ableColor = new Color32(255, 255, 255, 255);
    Color32 disableColor = new Color32(228, 35, 15, 255);

    private GameObject _effectPrefab;
    private Vector3 _effectOffset = new Vector3(0.8f, 1.1f, 0); // 이거 수치 두배되는데 확인해야됨...

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _audio = GetComponent<AudioSource>();
        _bullet = Resources.Load<GameObject>("Prefabs/Sprites/Bullet");
        _effectPrefab = Resources.Load<GameObject>("Prefabs/Sprites/Player/PlayerGunEffect");
        _aim = transform.GetChild(0);
        _aimSprite = _aim.GetComponent<SpriteRenderer>();
    }

    public void Reset()
    {
        _aimSprite.color = ableColor;
    }

    private void Update()
    {
        _aim.transform.position = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    public void Shoot(Vector3 playerPos)
    {
        Vector3 dir = _aim.position - playerPos - _delta;
        StartCoroutine(coShooting(dir, playerPos + _delta));
    }
    
    public IEnumerator coShooting(Vector3 dir, Vector3 pos)
    {
        _audio.Play();
        yield return new WaitForSeconds(0.35f);
        GameObject effect = Instantiate(_effectPrefab, transform);
        effect.transform.position += _effectOffset;
        yield return new WaitForSeconds(0.05f);
        GameObject go = Instantiate(_bullet);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.SetBullet(dir, pos, true);
        yield return new WaitForSeconds(0.8f);
        Destroy(effect);
    }

    public void ChangeState(bool canShoot)
    {
        try
        {
            _aimSprite.color = canShoot ? ableColor : disableColor;
        }
        catch (Exception)
        {
            Init();
            Reset();
        }
    }
}
