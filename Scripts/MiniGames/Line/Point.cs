using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퍼즐에서 연결 대상이 되는 포인트(노드).
/// id로 식별하고, 2D 좌표만 사용.
/// </summary>
public class Point : MonoBehaviour
{
    public int id;                    // 포인트 고유 id
    [HideInInspector] public Vector3 Position; // z=0으로 고정한 화면 좌표

    void Start()
    {
        Position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
