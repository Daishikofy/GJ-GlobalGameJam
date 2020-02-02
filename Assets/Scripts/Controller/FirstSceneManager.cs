using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneManager : MonoBehaviour
{
    public void startGame()
    {
       SceneManager.LoadScene("GameScene");       
    }
}
