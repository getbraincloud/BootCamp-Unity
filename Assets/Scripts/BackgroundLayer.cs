// Copyright 2022 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Constants.kBackgroundSpeed * Time.deltaTime);

        float width = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        if (transform.position.x < -width)
            transform.Translate(new Vector3(width * 2.0f, 0.0f, 0.0f));
    }
}
