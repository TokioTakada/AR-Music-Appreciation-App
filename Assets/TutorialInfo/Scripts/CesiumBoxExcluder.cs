using CesiumForUnity;
using UnityEngine;

[ExecuteInEditMode] // エディター上でも確認できるように追加
[RequireComponent(typeof(BoxCollider))]
public class CesiumBoxExcluder : CesiumTileExcluder
{
    BoxCollider m_BoxCollider;
    Bounds m_Bounds;

    protected override void OnEnable()
    {
        m_BoxCollider = gameObject.GetComponent<BoxCollider>();
        m_Bounds = new Bounds(m_BoxCollider.center, m_BoxCollider.size);

        base.OnEnable();
    }

    protected void Update()
    {
        m_Bounds.center = m_BoxCollider.center;
        m_Bounds.size = m_BoxCollider.size;
    }

    public bool CompletelyContains(Bounds bounds)
    {
        return Vector3.Min(this.m_Bounds.max, bounds.max) == bounds.max &&
               Vector3.Max(this.m_Bounds.min, bounds.min) == bounds.min;
    }

    public override bool ShouldExclude(Cesium3DTile tile)
    {
        if (!enabled)
        {
            return false;
        }

        return m_Bounds.Intersects(tile.bounds);
    }
}