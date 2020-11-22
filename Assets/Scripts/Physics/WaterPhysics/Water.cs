using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Water : MonoBehaviour 
{
    public PostProcessProfile Profile;
    [Range(0.0f, 100.0f)] public float Viscosity;
    [HideInInspector] public float WaterLevelY;

    private void Start() => WaterLevelY = transform.Find("WaterLevel").position.y;

}