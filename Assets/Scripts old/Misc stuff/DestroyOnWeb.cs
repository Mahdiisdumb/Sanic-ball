using UnityEngine;

public class DestroyOnWeb : MonoBehaviour
{
    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Destroy(gameObject);
        }
    }
}