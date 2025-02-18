using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character : UI_Scene
{
    enum GameObjects
    {
        HPBar,
        HPFill,
        Weapon,
        LifeBar,
        Bullet1,
        Bullet2,
        GaugeBar
    }

    PlayerController _player;
    private float smoothTime = 1;
    private MapScene scene;
    private Slider hpBarSlider;
    private Slider lifeBarSlider;
    private Slider gaugeBarSlider;
    private GameObject weaponChange;
    private Image hpImage;
    Coroutine updateCoroutine;

    private GameObject sword;
    private GameObject gun;

    public override void Init()
    {
        _player = FindObjectOfType<PlayerController>();

        BindObject(typeof(GameObjects));
        scene = FindObjectOfType<MapScene>();
        scene.Player.updateHp -= UpdateHp;
        scene.Player.updateHp += UpdateHp;

        scene.Player.changeWeapon -= WeaponChange;
        scene.Player.changeWeapon += WeaponChange;

        scene.Player.decreaseLife -= DecreaseLife;
        scene.Player.decreaseLife += DecreaseLife;

        scene.Player.gameOver -= GameOver;
        scene.Player.gameOver += GameOver;

        scene.AddTerraformingGauge -= UpdateTerraformingGauge;
        scene.AddTerraformingGauge += UpdateTerraformingGauge;

        scene.GetSkill -= GetSkill;
        scene.GetSkill += GetSkill;


        hpBarSlider = GetObject((int)GameObjects.HPBar).GetComponent<Slider>();
        weaponChange = GetObject((int)GameObjects.Weapon);
        lifeBarSlider = GetObject((int)GameObjects.LifeBar).GetComponent<Slider>();
        gaugeBarSlider = GetObject((int)GameObjects.GaugeBar).GetComponent<Slider>();
        hpImage = GetObject((int)GameObjects.HPFill).GetComponent<Image>();

        sword = weaponChange.transform.Find("Sword").gameObject;
        gun = weaponChange.transform.Find("Gun").gameObject;

        gaugeBarSlider.value = (Managers.Game.SaveData.terraformingGauge / 100f);

        //필요없는 오브젝트 비활성화 
        gun.SetActive(false);
    }

    private void Start()
    {
        Init();
    }

    private void UpdateHp(float updateHp)
    {
        float ratio = _player.Hp / Managers.Data.Player.hp;
        hpBarSlider.value = ratio;
        float green = ratio * 0.823f; // G 값을 0 ~ 210 사이 값으로 변환해 색 조절 
        hpImage.color = new Color(255/255f, green, 0, 1f);
        /* slider bar 천천히 업데이트 
         if (updateCoroutine != null) StopCoroutine(updateCoroutine); 
        updateCoroutine = StartCoroutine(SmoothUpdate(ratio, hpBarSlider));
        */
    }
    /*
    IEnumerator SmoothUpdate(float targetRatio, Slider slider)
    {
        float startValue = slider.value;
        float elapsedTime = 0;

        while(elapsedTime < smoothTime)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetRatio, elapsedTime / smoothTime);
            yield return null;
        }

        slider.value = targetRatio;
    }*/

    private void WeaponChange(Define.PlayerWeapon _weaponType, int shootCount)
    {
        if (_weaponType == Define.PlayerWeapon.Gun)
        {
            Debug.Log(shootCount);
            sword.SetActive(false);
            gun.SetActive(true);
            Charging(shootCount);
        }
        else if (_weaponType == Define.PlayerWeapon.Sword)
        {
            sword.SetActive(true);
            gun.SetActive(false);
        }
    }
    private void Charging(int shootCount)
    {
        if (shootCount > 0)
            GetObject((int)GameObjects.Bullet1).SetActive(true);
        else
            GetObject((int)GameObjects.Bullet1).SetActive(false);
       
        if (shootCount > 1)
            GetObject((int)GameObjects.Bullet2).SetActive(true);
        else GetObject((int)GameObjects.Bullet2).SetActive(false);
    }

    private void DecreaseLife()
    {
        float ratio = (float)_player.Life / 4;
        lifeBarSlider.value = ratio;
    }
    private void UpdateTerraformingGauge(float gauge)
    {
        Debug.Log("TerraforomingGauge Update" + gauge);
        gaugeBarSlider.value = gauge / 100;
    }

    private void GetSkill(Define.ForestEnemyType enemy)
    {
        Managers.UI.ShowPopupUI<UI_GetSkill>("GetSkillUI");
    }

    private void GameOver()
    {
        Debug.Log("Game Over !!");
    }
}
