
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] List<GameObject> hearts = new List<GameObject>();
    [SerializeField] Transform heartsContainer;
    [SerializeField] GameObject heartPrefab;

    void Awake()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        scoreText.text = "" + playerController.score;
        UpdateHearts();
    }

    void UpdateHearts()
    {
        int health = playerController.health;

        while (hearts.Count > health)
        {
            Destroy(hearts[hearts.Count - 1]);
            hearts.RemoveAt(hearts.Count - 1);
        }

        while (hearts.Count < health)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(heart);
        }
    }

}
