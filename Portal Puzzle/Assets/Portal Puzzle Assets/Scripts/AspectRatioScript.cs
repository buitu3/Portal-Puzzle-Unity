using UnityEngine;
using System.Collections;

public class AspectRatioScript : MonoBehaviour
{

    public float targetAspect;

    void Start()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Camera camera = GetComponent<Camera>();

        //if (scaleHeight < 1.0f)
        //{
        //    camera.orthographicSize = camera.orthographicSize / scaleHeight;
        //}
        //Screen.SetResolution(480, 700, false);


        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f; rect.height = scaleHeight;
            rect.x = 0; rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0; camera.rect = rect;
        }

        
        //Screen.SetResolution(350, 800, false);
        //Screen.fullScreen = false;
    }
}
