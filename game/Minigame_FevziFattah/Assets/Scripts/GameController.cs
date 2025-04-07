using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Bugs & Player")]
    [SerializeField] Bug standardBug;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerDeathScreen;
    PlayerController playerController;

    [Header("Unlocking & Items")]
    [SerializeField] private float scoreNeededToUnlockMultiplier = 2.25f;
    [SerializeField] private float firstItemUnlockScore = 300f;
    [SerializeField] private float firstBugUnlockScore = 500f;
    [SerializeField] GameObject itemSelectionPanel;
    [SerializeField] GameObject itemUpgradeText;
    [SerializeField] public List<GameObject> itemCards = new List<GameObject>();
    [SerializeField] GameObject bugUnlockPanel;
    [SerializeField] TextMeshProUGUI bugNameField;
    [SerializeField] TextMeshProUGUI bugDescriptionField;
    [SerializeField] Image bugIconField;

    [Header("Gameplay")]
    public bool isPaused = false;
    public List<Bug> allBugs = new List<Bug>();
    [SerializeField] List<Bug> unlockedBugs = new List<Bug>();
    int currentScore;
    private Coroutine spawnCoroutine;
    private float spawnRate = 1f;

    private bool selectingItem;
    private float nextBugUnlockScore;
    private float nextItemUnlockScore;

    void Awake(){
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (isPaused) Pause(false);
            else Pause(true);
        }
        currentScore = playerController.score;

        if (currentScore >= nextBugUnlockScore && allBugs.Count > 0)
        {
            unlockBug();
            nextBugUnlockScore *= scoreNeededToUnlockMultiplier;
        }

        if (currentScore >= nextItemUnlockScore)
        {
            openItemSelection(3);
            ChangeSpawnRate(spawnRate-0.2F);
            nextItemUnlockScore *= scoreNeededToUnlockMultiplier;
        }

        if (selectingItem && itemSelectionPanel.transform.childCount == 2)
        {
            selectingItem = false;
            closeItemSelection();
        }
    }

    void Start()
    {
        unlockBug(standardBug);
        nextBugUnlockScore = firstBugUnlockScore;
        nextItemUnlockScore = firstItemUnlockScore;
        //spawnCoroutine = StartCoroutine(SpawnBugCoroutine());
    }

    private IEnumerator SpawnBugCoroutine()
    {
        while (true)
        {
            if (!isPaused)
            {
                spawnBug();
            }
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Camera camera = Camera.main;
        float screenWidth = camera.orthographicSize * camera.aspect;
        float screenHeight = camera.orthographicSize;

        int edgeChoice = Random.Range(0, 4);
        switch (edgeChoice)
        {
            case 0: return new Vector3(-screenWidth - Random.Range(1f, 3f), Random.Range(-screenHeight, screenHeight), 0);
            case 1: return new Vector3(screenWidth + Random.Range(1f, 3f), Random.Range(-screenHeight, screenHeight), 0);
            case 2: return new Vector3(Random.Range(-screenWidth, screenWidth), screenHeight + Random.Range(1f, 3f), 0);
            case 3: return new Vector3(Random.Range(-screenWidth, screenWidth), -screenHeight - Random.Range(1f, 3f), 0);
            default: return Vector3.zero;
        }
    }

    void spawnBug()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Bug bug = Instantiate(unlockedBugs[Random.Range(0, unlockedBugs.Count)], spawnPosition, Quaternion.identity);
        Vector3 targetPosition = player.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        Vector3 direction = (targetPosition - bug.transform.position).normalized;

        bug.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
        bug.Project(direction * Random.Range(50, 300));
    }

    public void ChangeSpawnRate(float newRate)
    {
        spawnRate = newRate;
        if (!isPaused && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = StartCoroutine(SpawnBugCoroutine());
        }
    }

    public void Pause(bool shouldPause)
    {
        if (shouldPause)
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine); 
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            spawnCoroutine = StartCoroutine(SpawnBugCoroutine());
        }

        isPaused = shouldPause;
    }

    void openItemSelection(int itemCount)
    {
        List<int> randomNumbers = new List<int>();
        Pause(true);
        selectingItem = true;
        itemUpgradeText.SetActive(true);
        itemSelectionPanel.SetActive(true);

        for (int i = 0; i < itemCount; i++)
        {
            int randomNum = Random.Range(0, itemCards.Count);
            while(randomNumbers.Contains(randomNum) && itemCards.Count >= itemCount){
                randomNum = Random.Range(0, itemCards.Count);
            }
            randomNumbers.Add(randomNum);
            Instantiate(itemCards[randomNum], itemSelectionPanel.transform);
        }
    }

    void closeItemSelection()
    {
        Pause(false);
        selectingItem = false;
        itemUpgradeText.SetActive(false);
        itemSelectionPanel.SetActive(false);

        foreach (Transform child in itemSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void unlockBug(Bug bug = null)
    {
        if (allBugs.Count == 0) return;

        Bug selectedBug = bug ?? allBugs[Random.Range(0, allBugs.Count)];
        unlockedBugs.Add(selectedBug);
        allBugs.Remove(selectedBug);

        bugNameField.text = selectedBug.bugName;
        bugDescriptionField.text = selectedBug.description;
        bugIconField.sprite = selectedBug.gameObject.GetComponent<SpriteRenderer>().sprite;
        bugIconField.color = selectedBug.gameObject.GetComponent<SpriteRenderer>().color;
        bugIconField.material = selectedBug.gameObject.GetComponent<SpriteRenderer>().sharedMaterial;

        Pause(true);
        bugUnlockPanel.SetActive(true);
    }

    public void closeBugScreen()
    {
        Pause(false);
        bugUnlockPanel.SetActive(false);
    }

    public void PlayerDeath()
    {
        Pause(true);
        playerDeathScreen.SetActive(true);
    }

    public void Restart() => SceneManager.LoadScene(0);
}
