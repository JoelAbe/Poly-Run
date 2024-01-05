using System.Collections.Generic;
using UnityEngine;

public class RandomGoldCoinPlacement : MonoBehaviour
{
    public GameObject walkableArea;  // Reference to the parent "Walkable" GameObject
    public GameObject goldCoinPrefab;
    public Transform pickupsContainer;  // Assign the "PickupsContainer" GameObject in the Inspector

    public float placementRadius = 10.0f;
    public int NumberOfCoins = 200;

    public float yPlacementOffset = 0;
    public float minSpawnY = -1.0f;  // Minimum y-axis value for spawning
    public float maxSpawnY = 1.0f;   // Maximum y-axis value for spawning

    private void Start()
    {
        PlaceGoldCoins();
    }

    private void PlaceGoldCoins()
    {

        for (int i = 0; i < NumberOfCoins+1; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            Instantiate(goldCoinPrefab, randomPosition, Quaternion.identity, pickupsContainer);
        }
    }

    private Vector3 GetRandomPosition()
    {
        Transform[] subfolders = walkableArea.GetComponentsInChildren<Transform>();
        List<Vector3> validPositions = new List<Vector3>();

        foreach (Transform subfolder in subfolders)
        {
            if (subfolder != walkableArea.transform)  // Skip the main "Walkable" GameObject
            {
                foreach (Transform child in subfolder)
                {
                    // Check if the child has a valid position for coin placement
                    RaycastHit hit;
                    if (Physics.Raycast(child.position + Vector3.up * 10, Vector3.down, out hit, 20, LayerMask.GetMask("Walkable")))
                    {
                        Vector3 validPosition = hit.point + Vector3.up * (0.1f + GetGoldCoinPrefabOffset());

                        // Check if the y-coordinate of the valid position is within the desired range
                        if (validPosition.y >= minSpawnY && validPosition.y <= maxSpawnY)
                        {
                            validPositions.Add(validPosition);
                        }
                    }
                }
            }
        }

        if (validPositions.Count > 0)
        {
            // Select a random valid position
            return validPositions[Random.Range(0, validPositions.Count)];
        }

        // Fallback to center if no valid position found
        return walkableArea.transform.position + Random.insideUnitSphere * placementRadius;
    }

    private float GetGoldCoinPrefabOffset()
    {
        // To adjust the y axis positioning of the gold coins prefab
        return yPlacementOffset;
    }



}
