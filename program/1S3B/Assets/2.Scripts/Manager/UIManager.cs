using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class UIManager : Manager
{
    private Player player;
    private PopUpController popUpController;
    private SoundSystemManager soundManager;

    [Header("UI")]
    public TMP_Text goldUIText;
    public TMP_Text shopGoldText;
    public Image seasonImage;
    public SpriteLibraryAsset spriteLibraryAsset;    

    [Header("Image")]
    public GameObject sleepInfoUI;
    public QuickSlotUI quickSlotUI;
    public InventoryUI inventoryUI;
    public ShopUI shopUI;
    public GameObject keepOutInfo;
    public GameObject preferencesUI;

    [Header("EnergyBar")]
    [SerializeField] private GameObject tired;
    [SerializeField] private Slider energyBar;
    private TMP_Text energyText;

    [Header("MiniMap")]
    public Image miniMap;
    public GameObject playerObj;//������ RectTransform���� �����Ҵ��ϸ� �ȵ�
    private RectTransform playerImage;

    private int maxEnergy;

    public override void Init(GameManager gm)
    {
        base.Init(gm);

        player = gameManager.Player;
        popUpController = gameManager.PopUpController;
        soundManager = gameManager.SoundManager;

        energyText = energyBar.GetComponentInChildren<TMP_Text>();
        maxEnergy = player.playerMaxEnergy;
        EnengyBarSetting();
    }

    private void Start()
    {        
        playerImage = playerObj.GetComponent<RectTransform>();
        playerImage.anchoredPosition = new Vector2(470, 130);//�����Ҷ� ��ġ ���߿� �����ؼ�

        quickSlotUI.Init(gameManager, this, player);
        inventoryUI.Init(gameManager, this, player);
        shopUI.Init();

        GameManager.Instance.DayCycleHandler.changeSeasonAction += ChangeSeasonImage;
    }

    //public void EquipIconChange(PlayerEquipmentType type)
    //{
    //    equipIcon.sprite = libraryAsset.GetSprite("Equip", type.ToString());
    //}
    //public void EquipIconChange(string type)
    //{
    //    equipIcon.sprite = libraryAsset.GetSprite("Equip", type);
    //}

    public void ChangeSeasonImage(Season season)
    {
        seasonImage.sprite = spriteLibraryAsset.GetSprite("Season", season.ToString());
    }

    public void MiniMapPosition(PlayerMap map)
    {
        player.playerMap = map;

        switch (map)
        {
            case PlayerMap.Farm:
                playerImage.anchoredPosition = new Vector2(620, 200);
                break;
            case PlayerMap.FarmToTown:
                playerImage.anchoredPosition = new Vector2(267, 134);
                break;
            case PlayerMap.Town:
                playerImage.anchoredPosition = new Vector2(50, 134);
                break;
            case PlayerMap.TownToForest:
                playerImage.anchoredPosition = new Vector2(-176, 138);
                break;
            case PlayerMap.Forest:
                playerImage.anchoredPosition = new Vector2(-501, 151);
                break;
            case PlayerMap.FarmToForest:
                playerImage.anchoredPosition = new Vector2(29, 359);
                break;
            case PlayerMap.Beach:
                playerImage.anchoredPosition = new Vector2(37, -129);
                break;
            case PlayerMap.BeachR:
                playerImage.anchoredPosition = new Vector2(375, -104);
                break;
            case PlayerMap.BeachL:
                playerImage.anchoredPosition = new Vector2(-488, -167);
                break;
            case PlayerMap.BossRoom:
                playerImage.anchoredPosition = new Vector2(-701, 96);
                break;
            case PlayerMap.Island:
                playerImage.anchoredPosition = new Vector2(142, -384);
                break;
            case PlayerMap.Quarry:
                playerImage.anchoredPosition = new Vector2(693, -296);
                break;
            case PlayerMap.Home:
                playerImage.anchoredPosition = new Vector2(470, 130);
                break;
        }
    }

    private void EnengyBarSetting()
    {
        energyBar.maxValue = maxEnergy;
        energyBar.minValue = 0;

        EnergyBarUpdate(maxEnergy);

        energyText.gameObject.SetActive(false);
        tired.SetActive(false);
    }
    public void EnergyBarUpdate(int playerEnergy)
    {
        if (playerEnergy > maxEnergy)
            playerEnergy = maxEnergy;

        energyBar.value = playerEnergy;
        energyText.text = playerEnergy + "/" + maxEnergy;
    }
    public void TiredIconOnOff(bool value)
    {
        tired.SetActive(value);
    }

    public void UpdateGoldUI(int playerGold)
    {
        goldUIText.text = playerGold.ToString();
        shopGoldText.text = playerGold.ToString();
    }
    
    public void PreferencesUIOnOff()
    {
        if (!preferencesUI.activeSelf)
        {
            soundManager.GameAudioClipPlay((int)MainAudioClip.Click);
            popUpController.UIOn(preferencesUI);
        }
        else
        {
            soundManager.GameAudioClipPlay((int)MainAudioClip.Close);
            popUpController.UIOff(preferencesUI);
        }
    }
}
