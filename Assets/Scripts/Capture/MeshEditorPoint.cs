using UnityEngine;

public class MeshEditorPoint : MonoBehaviour
{
    //顶点id，（顶点初始位置转字符串）
    [HideInInspector]
    public string pointId;

    //记录坐标点上一次移动的位置，用于判断控制点是否移动
    private Vector3 _lastPosition;

    public delegate void MoveDelegate(string pid, Vector3 pos);

    //控制点移动时的回调
    public MoveDelegate onMove = null;

    // Use this for initialization
    private void Start()
    {
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if(transform.position == _lastPosition)
        {
            return;
        }

        onMove?.Invoke(pointId, transform.localPosition);

        _lastPosition = transform.position;
    }
}