using UnityEngine;

public class WD_Manager : MonoBehaviour
{
    public static WD_Manager Instance { get; private set; }
    public void Awake() => Instance = this;

    [Header("padding settings")]
    public float titleBarHeight = 0.7f;
    public float sideEdge = 0.27f;
    public float bottomEdge = 0.2f;
}