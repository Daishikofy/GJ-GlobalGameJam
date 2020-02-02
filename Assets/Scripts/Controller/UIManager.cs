#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField]
    GameObject koPanel;
    [SerializeField]
    Image goodKarma;
    [SerializeField]
    Image badKarma;
    [Space]
    [SerializeField]
    Image[] life;
    [SerializeField]
    TextMeshProUGUI level;
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
        player.updateKarma.AddListener(updateKarma);
        player.updateLife.AddListener(updateLife);
        GameManager.Instance.updateLevel.AddListener(updateLevel);
        koPanel.SetActive(false);
    }

    private void updateLife(int value)
    {
        if (life[value].IsActive())
            life[value + 1].enabled = false;
        else
            life[value].enabled = true;
    }
    private void updateKarma(int value)
    {
        //MAX KARMA
        float maxValue = 50;
        Image currentImage;
        if (value > 0)
        {
            currentImage = goodKarma;
            badKarma.fillAmount = 0f;
        }
        else
        {
            currentImage = badKarma;
            goodKarma.fillAmount = 0f;
        }
        float newFill = Mathf.Abs(value) / maxValue;   
        currentImage.fillAmount = newFill;
    }
    private void updateLevel(int value)
    {
        level.text = "Level: " + value.ToString();
    }

    private IEnumerator fillUI(Image image, float newFill, float speed)
    {
        float remaining = Mathf.Abs(image.fillAmount - newFill);
        while (remaining > float.Epsilon)
        {
            float newPosition = image.fillAmount + newFill * Time.deltaTime * speed;
            image.fillAmount = newPosition;
            remaining = Mathf.Abs(image.fillAmount - newFill);

            yield return null;
        }
    }

    public void showKoPanel()
    {
        koPanel.SetActive(true);
    }
}
