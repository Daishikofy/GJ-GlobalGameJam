using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour, IInteractible
{
    [SerializeField]
    GameObject icon;
    Collider2D detection;
    private void Start()
    {       
        detection = GetComponent<CircleCollider2D>();
        detection.isTrigger = true;
        icon.SetActive(false);
    }
    public void onInteraction(PlayerController player)
    {
        Debug.Log("I am the elevator");
        GameManager.Instance.changeLevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            icon.SetActive(true);
            LeanTween.scale(icon, Vector3.one, 0.5f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {     
            LeanTween.scale(icon, Vector3.zero, 0.5f);
            StartCoroutine(wait(0.5f));
        }
    }

    private IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
        icon.SetActive(false);
    }
}
