using UnityEngine;

public class GoldCoins : MonoBehaviour
{
    public int goldValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        // Increment the score by the coin value
        ScoreManager.Instance.AddScore(goldValue);

        // Destroy the gold coin's parent GameObject
        Destroy(transform.parent.gameObject);
    }
}
