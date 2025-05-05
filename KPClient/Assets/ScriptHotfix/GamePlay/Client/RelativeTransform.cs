using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class RelativeTransform
{
    public Vector3 relativePosition;
    public Quaternion relativeRotation;
    public Vector3 relativeScale;

    public RelativeTransform(Transform parent, Transform child)
    {
        relativePosition = parent.InverseTransformPoint(child.position);
        relativeRotation = Quaternion.Inverse(parent.rotation) * child.rotation;
        relativeScale = new Vector3(
            child.localScale.x / parent.localScale.x,
            child.localScale.y / parent.localScale.y,
            child.localScale.z / parent.localScale.z);
    }

    [Button]
    public void ApplyTo(Transform parent, Transform child)
    {
        child.position = parent.TransformPoint(relativePosition);
        child.rotation = parent.rotation * relativeRotation;
        child.localScale = new Vector3(
            relativeScale.x * parent.localScale.x,
            relativeScale.y * parent.localScale.y,
            relativeScale.z * parent.localScale.z);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static RelativeTransform FromJson(string json)
    {
        return JsonUtility.FromJson<RelativeTransform>(json);
    }
}