using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSphereAlways : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
    }
}
