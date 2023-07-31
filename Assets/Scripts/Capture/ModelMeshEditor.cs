using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class ModelMeshEditor : MonoBehaviour
{
    //控制点的大小
    public float pointScale = 1.0f;
    private float _lastPointScale = 1.0f;

    private Mesh _mesh;

    //顶点列表
    private List<Vector3> _positionList = new List<Vector3>();

    //顶点控制物体列表
    private readonly List<GameObject> _positionObjList = new List<GameObject>();

    /// <summary>
    /// key:顶点字符串
    /// value:顶点在列表中的位置
    /// </summary>
    private readonly Dictionary<string, List<int>> _pointMap = new Dictionary<string, List<int>>();

    // Use this for initialization
    private void Start()
    {
        _lastPointScale = pointScale;
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        CreateEditorPoint();
    }

    //创建控制点
    public void CreateEditorPoint()
    {
        _positionList = new List<Vector3>(_mesh.vertices);

        for(var i = 0; i < _mesh.vertices.Length; i++)
        {
            var vstr = Vector2String(_mesh.vertices[i]);

            if(!_pointMap.ContainsKey(vstr))
            {
                _pointMap.Add(vstr, new List<int>());
            }

            _pointMap[vstr].Add(i);
        }

        foreach(var key in _pointMap.Keys)
        {
            var go = (GameObject) Resources.Load("Prefabs/MeshEditor/MeshEditorPoint");
            go = Instantiate(go, transform, true);
            go.transform.localPosition = String2Vector(key);
            go.transform.localScale = new Vector3(1f, 1f, 1f);

            var editorPoint = go.GetComponent<MeshEditorPoint>();
            editorPoint.onMove = PointMove;
            editorPoint.pointId = key;

            _positionObjList.Add(go);
        }
    }

    //顶点物体被移动时调用此方法
    public void PointMove(string pointId, Vector3 position)
    {
        if(!_pointMap.ContainsKey(pointId))
        {
            return;
        }

        var list = _pointMap[pointId];

        for(var i = 0; i < list.Count; i++)
        {
            _positionList[list[i]] = position;
        }

        _mesh.vertices = _positionList.ToArray();
        _mesh.RecalculateNormals();
    }

    // Update is called once per frame
    private void Update()
    {
        //检测控制点尺寸是否改变
        if(Math.Abs(_lastPointScale - pointScale) > 0.1f)
        {
            _lastPointScale = pointScale;

            for(var i = 0; i < _positionObjList.Count; i++)
            {
                _positionObjList[i].transform.localScale = new Vector3(pointScale, pointScale, pointScale);
            }
        }
    }

    private static string Vector2String(Vector3 v)
    {
        var str = new StringBuilder();
        str.Append(v.x).Append(",").Append(v.y).Append(",").Append(v.z);
        return str.ToString();
    }

    private static Vector3 String2Vector(string vstr)
    {
        try
        {
            var strings = vstr.Split(',');
            return new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
            return Vector3.zero;
        }
    }
}