using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<EnemyMovement>().GameRestart();
        }
    }
}