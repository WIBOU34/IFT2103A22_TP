using System;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private const ushort MAX_NBR_NEIGHBORS = 4;
    public Vector2 dirConnectFromParent;
    public WallType type;
    public Wall[] neighbors = new Wall[MAX_NBR_NEIGHBORS];

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(WallType type, Vector2 direction)
    {
        this.type = type;
        this.dirConnectFromParent = direction;

        if (this.type == WallType.TOWER)
        {
            this.gameObject.name = "Tower";
            this.gameObject.transform.localScale = new Vector3(2, 4, 2);
        }
        else if (this.type == WallType.STRAIGHT)
        {
            this.gameObject.name = "Center";
            Vector3 scale;
            if (direction == Vector2.up || direction == Vector2.down)
                scale = new Vector3(1, 3, 6);
            else
                scale = new Vector3(6, 3, 1);
            this.gameObject.transform.localScale = scale;
        }
    }

    public void SetPositionFromNeighbor(Wall neighbor)
    {
        Vector3 neighborConnectPosition = neighbor.GetWallPositionForNeighborConnection(this.dirConnectFromParent);
        Vector3 center = GetWallCenterFromNeighborConnection(this.dirConnectFromParent, neighborConnectPosition);

        if (type == WallType.STRAIGHT)
        {
            this.gameObject.transform.position = new Vector3(center.x, 1.5f, center.z);
        }
        else if (type == WallType.TOWER)
        {
            this.gameObject.transform.position = new Vector3(center.x, 2, center.z);
        }
    }

    public bool AddNeighbor(Vector2 direction, Wall possibleNeighbor)
    {
        if (IsNeighborAvailable(direction))
            return false;

        SetNeighbor(direction, possibleNeighbor);
        return true;
    }

    Vector3 GetWallPositionForNeighborConnection(Vector2 direction)
    {
        Bounds currentBounds = this.gameObject.GetComponent<Collider>().bounds;
        Vector3 center = currentBounds.center;
        Vector3 extents = currentBounds.extents;

        if (direction.Equals(Vector2.up))
        {
            Vector3 centerUp = new Vector3(center.x, center.y, center.z + extents.z);
            return centerUp;
        }
        else if (direction.Equals(Vector2.left))
        {
            Vector3 centerLeft = new Vector3(center.x - extents.x, center.y, center.z);
            return centerLeft;
        }
        else if (direction.Equals(Vector2.down))
        {
            Vector3 centerDown = new Vector3(center.x, center.y, center.z - extents.z);
            return centerDown;
        }
        else if (direction.Equals(Vector2.right))
        {
            Vector3 centerRight = new Vector3(center.x + extents.x, center.y, center.z);
            return centerRight;
        }
        throw new IndexOutOfRangeException("The direction is not supported");
    }

    Vector3 GetWallCenterFromNeighborConnection(Vector2 direction, Vector3 neighborConnectPosition)
    {
        Bounds currentBounds = this.gameObject.GetComponent<Collider>().bounds;
        Vector3 extents = currentBounds.extents;



        if (direction.Equals(Vector2.up))
        {
            Vector3 centerUp = new Vector3(neighborConnectPosition.x, neighborConnectPosition.y, neighborConnectPosition.z + extents.z);
            return centerUp;
        }
        else if (direction.Equals(Vector2.left))
        {
            Vector3 centerLeft = new Vector3(neighborConnectPosition.x - extents.x, neighborConnectPosition.y, neighborConnectPosition.z);
            return centerLeft;
        }
        else if (direction.Equals(Vector2.down))
        {
            Vector3 centerDown = new Vector3(neighborConnectPosition.x, neighborConnectPosition.y, neighborConnectPosition.z - extents.z);
            return centerDown;
        }
        else if (direction.Equals(Vector2.right))
        {
            Vector3 centerRight = new Vector3(neighborConnectPosition.x + extents.x, neighborConnectPosition.y, neighborConnectPosition.z);
            return centerRight;
        }
        throw new IndexOutOfRangeException("The direction is not supported");
    }

    public bool IsNeighborAvailable(Vector2 direction)
    {
        if (type == WallType.STRAIGHT) // Only 2 corners in the same direction
        {
            // Verify if we are looking for a corner in the same direction as the wall direction
            if (!RemoveNegation(this.dirConnectFromParent).Equals(RemoveNegation(direction)))
            {
                return false;
            }
        }
        // Verify if neighbor is free (e.g. null)
        try
        {
            Wall neighbor = GetNeighbor(direction);
            if (neighbor == null)
                return true;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }

        return false;
    }

    private Vector2 RemoveNegation(Vector2 other)
    {
        return new Vector2(Mathf.Abs(other.x), Mathf.Abs(other.y));
    }

    Wall GetNeighbor(Vector2 direction)
    {
        return neighbors[GetNeighborIndex(direction)];
    }

    void SetNeighbor(Vector2 direction, Wall neighbor)
    {
        neighbors[GetNeighborIndex(direction)] = neighbor;
    }

    // Vector2.up = 0
    // Vector2.left = 1
    // Vector2.down = 2
    // Vector2.right = 3
    private ushort GetNeighborIndex(Vector2 direction)
    {
        if (direction.Equals(Vector2.up))
        {
            return 0;
        }
        else if (direction.Equals(Vector2.left))
        {
            return 1;
        }
        else if (direction.Equals(Vector2.down))
        {
            return 2;
        }
        else if (direction.Equals(Vector2.right))
        {
            return 3;
        }
        throw new IndexOutOfRangeException("The direction is not supported");
    }

    // Destroyed actions
    public void PrepareForDelete()
    {
        for (ushort i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (neighbors[i] != null)
                neighbors[i].NeighborDestroyed(this);
        }
    }

    void NeighborDestroyed(Wall other)
    {
        for (ushort i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (neighbors[i] != null && neighbors[i] == other)
                neighbors[i] = null;
        }
    }
}
