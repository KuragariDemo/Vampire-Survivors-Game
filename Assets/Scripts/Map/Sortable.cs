using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{

    protected SpriteRenderer sorted;
    public bool sortingActive = true; 
    public float minimumDistance = 0.2f; 
    int lastSortOrder = 0;

    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (!sorted) return;
        int newSortOrder = (int)(-transform.position.y / minimumDistance);
        if (lastSortOrder != newSortOrder) sorted.sortingOrder = newSortOrder;
    }
}
