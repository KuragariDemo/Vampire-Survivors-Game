using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITreasureChest : MonoBehaviour
{

    public static UITreasureChest instance;
    PlayerCollector collector;
    TreasureChest currentChest;
    TreasureChestDropProfile dropProfile;

    [Header("Visual Elements")]
    public GameObject openingVFX;
    public GameObject beamVFX;
    public GameObject fireworks;
    public GameObject doneButton;
    public GameObject curvedBeams;
    public List<ItemDisplays> items;
    Color originalColor = new Color32(0x42, 0x41, 0x87, 255);

    [Header("UI Elements")]
    public GameObject chestCover;
    public GameObject chestButton;

    [Header("UI Components")]
    public Image chestPanel;
    public TextMeshProUGUI coinText;
    private float coins;

    // Internal states
    private List<Sprite> icons = new List<Sprite>();
    private bool isAnimating = false;
    private Coroutine chestSequenceCoroutine;

    //audio
    private AudioSource audiosource;
    public AudioClip pickUpSound;

    [System.Serializable]
    public struct ItemDisplays
    {
        public GameObject beam;
        public Image spriteImage;
        public GameObject sprite;
        public GameObject weaponBeam;
    }

    private void Update()
    {
        if (isAnimating && Input.GetButtonDown("Cancel"))
        {
            SkipToRewards();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPressButton(chestButton);
            TryPressButton(doneButton);
        }
    }

    private void TryPressButton(GameObject buttonObj)
    {
        if (buttonObj.activeInHierarchy)
        {
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null && btn.interactable)
            {
                btn.onClick.Invoke();
            }
        }
    }

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        gameObject.SetActive(false);

        // Ensure only 1 instance can exist in the scene
        if (instance != null && instance != this)
        {
            Debug.LogWarning("More than 1 UI Treasure Chest is found. It has been deleted.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static void Activate(PlayerCollector collector, TreasureChest chest) {
        if(!instance) Debug.LogWarning("No treasure chest UI GameObject found.");

        instance.collector = collector;
        instance.currentChest = chest;
        instance.dropProfile = chest.GetCurrentDropProfile();
        
        GameManager.instance.ChangeState(GameManager.GameState.TreasureChest);
        instance.gameObject.SetActive(true);
    }

    public IEnumerator Open()
    {
        if (dropProfile.hasFireworks)
        {
            isAnimating = false; 
            StartCoroutine(FlashWhite(chestPanel, 5)); 
            fireworks.SetActive(true);
            yield return new WaitForSecondsRealtime(dropProfile.fireworksDelay);
        }

        isAnimating = true; 

        
        if (dropProfile.hasCurvedBeams)
        {
            StartCoroutine(ActivateCurvedBeams(dropProfile.curveBeamsSpawnTime));
        }

        
        StartCoroutine(HandleCoinDisplay(Random.Range(dropProfile.minCoins, dropProfile.maxCoins)));

        DisplayerBeam(dropProfile.noOfItems);
        openingVFX.SetActive(true);
        beamVFX.SetActive(true);

        yield return new WaitForSecondsRealtime(dropProfile.animDuration); 
        openingVFX.SetActive(false);
    }

    IEnumerator ActivateCurvedBeams(float spawnTime)
    {
        yield return new WaitForSecondsRealtime(spawnTime);
        curvedBeams.SetActive(true);
    }

    
    private IEnumerator FlashWhite(Image image, int times, float flashDuration = 0.2f)
    {
        originalColor = image.color;


        for (int i = 0; i < times; i++)
        {
            image.color = Color.white;
            yield return new WaitForSecondsRealtime(flashDuration);

            image.color = originalColor;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    public void DisplayerBeam(float noOfBeams)
    {
        int delayedStartIndex = Mathf.Max(0, (int)noOfBeams - dropProfile.delayedBeams); 


        for (int i = 0; i < delayedStartIndex; i++)
        {
            SetupBeam(i);
        }


        if (dropProfile.delayedBeams > 0)
            StartCoroutine(ShowDelayedBeams(delayedStartIndex, (int)noOfBeams));

        StartCoroutine(DisplayItems(noOfBeams));
    }

 
    private void SetupBeam(int index)
    {
        items[index].weaponBeam.SetActive(true);
        items[index].beam.SetActive(true);
        items[index].spriteImage.sprite = icons[index];
        items[index].beam.GetComponent<Image>().color = dropProfile.beamColors[index];
    }


    private IEnumerator ShowDelayedBeams(int startIndex, int endIndex)
    {
        yield return new WaitForSecondsRealtime(dropProfile.delayTime);

        for (int i = startIndex; i < endIndex; i++)
        {
            SetupBeam(i);
        }
    }

    private IEnumerator DisplayItems(float noOfBeams)
    {
        yield return new WaitForSecondsRealtime(dropProfile.animDuration);

        if (noOfBeams == 5)
        {

            items[0].weaponBeam.SetActive(false);
            items[0].sprite.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);


            for (int i = 1; i <= 2; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);

       
            for (int i = 3; i <= 4; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }
        else
        {

            for (int i = 0; i < noOfBeams; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }
    }


    public void Begin()
    {
        chestCover.SetActive(false);
        chestButton.SetActive(false);
        chestSequenceCoroutine = StartCoroutine(Open());
        audiosource.clip = dropProfile.openingSound;
        audiosource.Play();
    }


    IEnumerator HandleCoinDisplay(float maxCoins)
    {
        coinText.gameObject.SetActive(true);
        float elapsedTime = 0;
        coins = maxCoins;


        while (elapsedTime < maxCoins) 
        {
            elapsedTime += Time.unscaledDeltaTime * 20f;
            coinText.text = string.Format("{0:F2}", elapsedTime);
            yield return null;
        }
        
        yield return new WaitForSecondsRealtime(2f);
        doneButton.SetActive(true);
    }

    public void CloseUI()
    {

        collector.AddCoins(coins);

        chestCover.SetActive(true);
        chestButton.SetActive(true);
        icons.Clear();
        beamVFX.SetActive(false);
        coinText.gameObject.SetActive(false);
        gameObject.SetActive(false);
        doneButton.SetActive(false);
        fireworks.SetActive(false);
        curvedBeams.SetActive(false);
        ResetDisplay();


        audiosource.clip = pickUpSound;
        audiosource.time = 0f;
        audiosource.Play();

        isAnimating = false;

        GameManager.instance.ChangeState(GameManager.GameState.Gameplay);
        currentChest.NotifyComplete();
    }


    public static void NotifyItemReceived(Sprite icon)
    {

        if(instance) instance.icons.Add(icon);
        else Debug.LogWarning("No instance of UITreasureChest exists. Unable to update treasure chest UI.");
    }


    private void ResetDisplay()
    {
        foreach (var item in items)
        {
            item.beam.SetActive(false);
            item.sprite.SetActive(false);
            item.spriteImage.sprite = null;

        }
        dropProfile = null;
        icons.Clear();
    }

    private void SkipToRewards()
    {
        if (chestSequenceCoroutine != null)
            StopCoroutine(chestSequenceCoroutine);

        StopAllCoroutines(); 


        for (int i = 0; i < icons.Count; i++)
        {
            SetupBeam(i);
            items[i].weaponBeam.SetActive(false);
            items[i].sprite.SetActive(true);
        }


        coinText.gameObject.SetActive(true);
        coinText.text = coins.ToString("F2");
        doneButton.SetActive(true);
        openingVFX.SetActive(false);
        isAnimating = false;
        chestPanel.color = originalColor;


        if (audiosource != null && dropProfile.openingSound != null)
        {
            audiosource.clip = dropProfile.openingSound;

            float skipToTime = Mathf.Max(0, audiosource.clip.length - 3.55f); 
            audiosource.time = skipToTime;
            audiosource.Play();
        }
    }


}
