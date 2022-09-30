using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSignAnimation : MonoBehaviour
{
    bool moveToCenter = true;

    float smoothTime = 0.4f;
    float currentVelocity = 0.0f;

    void Update()
    {
        if (transform.localPosition.x >= -0.8f)
        {
            moveToCenter = false;
        }

        if (moveToCenter)
        {
            MoveTo(0);
        }
        else
        {
            MoveTo(1000);
        }
    }

    private void MoveTo(int xPosition)
    {
        float newPosition = Mathf.SmoothDamp(transform.localPosition.x, xPosition, ref currentVelocity, smoothTime);
        transform.localPosition = new Vector3(newPosition, transform.localPosition.y, transform.localPosition.z);
    }
}
