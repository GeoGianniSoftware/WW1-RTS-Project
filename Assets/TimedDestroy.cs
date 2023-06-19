using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float time = 25f;
    public bool StartWhenParentIsDestroyed = false;

    private void Update() {
        if (StartWhenParentIsDestroyed && transform.parent != null)
            return;

        time -= Time.deltaTime;

        if (time <= 0)
            Destroy(gameObject);

    }
}
