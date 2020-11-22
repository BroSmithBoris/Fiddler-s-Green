using UnityEngine;

public class ObstacleCheck : MonoBehaviour
{
    [HideInInspector] public bool IsObstacle;

    private void OnTriggerEnter(Collider other) => IsObstacle = true;

    private void OnTriggerExit(Collider other) => IsObstacle = false;
}