using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public float gameOverDistance = 1.2f;

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= gameOverDistance)
        {
            GameManager.Instance.GameOver();
        }
    }
}
