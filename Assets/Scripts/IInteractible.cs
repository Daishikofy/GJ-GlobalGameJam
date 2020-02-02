using UnityEngine;
using UnityEditor;

public interface IInteractible 
{
    void onInteraction(PlayerController player);
}