using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1. Sửa chữ này thành GameManager (bỏ chữ n thừa)
    public static GameManager instance; 

    [HideInInspector]
    public Vector2 lastCheckpointPos; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}