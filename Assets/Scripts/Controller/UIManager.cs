#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField]
    TextMeshProUGUI karma;
    [SerializeField]
    TextMeshProUGUI life;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    TextMeshProUGUI healthAttackBar;
    #region Singleton
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public static UIManager Instance { get { return instance; } }
    #endregion Singleton

    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.updateHealMeter.AddListener(updateHealMeter);
        player.updateKarma.AddListener(updateKarma);
        player.updateLife.AddListener(updateLife);
        GameManager.Instance.updateLevel.AddListener(updateLevel);
    }

    private void updateHealMeter(int value)
    {
        healthAttackBar.text = "HealthAttack: " + value.ToString();
    }
    private void updateLife(int value)
    {
        life.text = "Life: " + value.ToString();
    }
    private void updateKarma(int value)
    {
        karma.text = "Karma: " + value.ToString();
    }
    private void updateLevel(int value)
    {
        level.text = "Level: " + value.ToString();
    }
}
