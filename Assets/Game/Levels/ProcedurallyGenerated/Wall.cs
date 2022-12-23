using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Wall visitedWall = null;
    public const ushort MAX_NBR_NEIGHBORS = 4;
    public Vector2 dirConnectFromParent;
    public WallType type;
    public Wall[] neighbors = new Wall[MAX_NBR_NEIGHBORS];
    public bool isInvisible = false;

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
        MakeVisible();

        if (this.type == WallType.TOWER)
        {
            this.gameObject.name = "Tower";
            this.gameObject.transform.localScale = new Vector3(2, 4, 2);
        }
        else if (this.type == WallType.STRAIGHT || this.type == WallType.INVISIBLE)
        {
            if (this.type == WallType.INVISIBLE)
            {
                this.gameObject.name = "Invisible";
                MakeInvisible();
            }
            else
            {
                this.gameObject.name = "Center";
            }
            Vector3 scale;
            if (direction == Vector2.up || direction == Vector2.down)
                scale = new Vector3(1, 3, 6);
            else
                scale = new Vector3(6, 3, 1);
            this.gameObject.transform.localScale = scale;
        }
    }

    public Vector3 GetPositionFromNeighbor(Wall neighbor)
    {
        Vector2 posNeighbor = new Vector2(neighbor.gameObject.transform.position.x, neighbor.gameObject.transform.position.z);
        Vector2 posCurrentWall = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z);
        Vector2 dirFromHereToTarget = GetVectorDirection(posNeighbor, posCurrentWall);
        Vector2 dirFromTargetToHere = dirFromHereToTarget * -1;

        return GetPositionFromNeighbor(neighbor, dirFromHereToTarget, dirFromTargetToHere);
    }

    public Vector3 GetPositionFromNeighbor(Wall neighbor, Vector2 dirFromHereToTarget, Vector2 dirFromTargetToHere)
    {
        Vector3 neighborConnectPosition = neighbor.GetWallConnectPositionForNeighbor(dirFromTargetToHere);
        Vector3 center = GetWallCenterFromNeighborConnection(dirFromHereToTarget, neighborConnectPosition);

        if (type == WallType.STRAIGHT || type == WallType.INVISIBLE)
        {
            return new Vector3(center.x, 1.5f, center.z);
        }
        else
        {
            return new Vector3(center.x, 2, center.z);
        }
    }

    public void SetPositionFromNeighbor(Wall neighbor, Vector2 direction)
    {
        Vector2 reverseDirection = direction * -1;
        this.gameObject.transform.position = GetPositionFromNeighbor(neighbor, direction, reverseDirection);
    }

    public bool AddBothAsNeighbor(Vector2 currentWallDirection, Wall possibleNeighbor)
    {
        Vector2 reverseDirection = currentWallDirection * -1;
        bool result = AddNeighbor(currentWallDirection, possibleNeighbor);
        if (result)
            result = possibleNeighbor.AddNeighbor(reverseDirection, this);
        return result;
    }

    public bool AddNeighbor(Vector2 direction, Wall possibleNeighbor)
    {
        if (!IsNeighborAvailable(direction))
            return false;

        SetNeighbor(direction, possibleNeighbor);
        if (type == WallType.TOWER)
            MakeInvisibleIfNoVisibleNeighbor();
        return true;
    }

    public void AddNeighborIfNeighbor(Wall otherWall)
    {
        Vector2 posOtherWall = new Vector2(otherWall.gameObject.transform.position.x, otherWall.gameObject.transform.position.z);
        Vector2 posCurrentWall = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z);
        Vector2 dirFromHereToTarget = GetVectorDirection(posOtherWall, posCurrentWall);
        if (!ValidateDirectionIsValid(dirFromHereToTarget))
            return;
        Vector2 dirFromTargetToHere = dirFromHereToTarget * -1;

        // validates if the direction is a valid direction (90 degrees and is available) for both wall
        if (IsNeighborAvailable(dirFromHereToTarget) && otherWall.IsNeighborAvailable(dirFromTargetToHere))
        {
            // validates if the walls require to be moved to be neighbors
            if (GetPositionFromNeighbor(otherWall) == this.gameObject.transform.position)
            {
                // Adds a neighbor
                otherWall.SetNeighbor(dirFromTargetToHere, this);
                SetNeighbor(dirFromHereToTarget, otherWall);
                otherWall.MakeInvisibleIfNoVisibleNeighbor();
                MakeInvisibleIfBlocage();
            }
        }
    }

    private static bool ValidateDirectionIsValid(Vector2 direction)
    {
        return direction.Equals(Vector2.up) || direction.Equals(Vector2.down) || direction.Equals(Vector2.left) || direction.Equals(Vector2.right);
    }

    private static Vector2 GetVectorDirection(Vector2 target, Vector2 origin)
    {
        return (target - origin).normalized;
    }

    Vector3 GetWallConnectPositionForNeighbor(Vector2 direction)
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
        throw new System.IndexOutOfRangeException("The direction is not supported");
    }

    Vector3 GetWallCenterFromNeighborConnection(Vector2 direction, Vector3 neighborConnectPosition)
    {
        Bounds currentBounds = this.gameObject.GetComponent<Collider>().bounds;
        Vector3 extents = currentBounds.extents;

        if (direction.Equals(Vector2.up))
        {
            Vector3 centerUp = new Vector3(neighborConnectPosition.x, neighborConnectPosition.y, neighborConnectPosition.z - extents.z);
            return centerUp;
        }
        else if (direction.Equals(Vector2.left))
        {
            Vector3 centerLeft = new Vector3(neighborConnectPosition.x + extents.x, neighborConnectPosition.y, neighborConnectPosition.z);
            return centerLeft;
        }
        else if (direction.Equals(Vector2.down))
        {
            Vector3 centerDown = new Vector3(neighborConnectPosition.x, neighborConnectPosition.y, neighborConnectPosition.z + extents.z);
            return centerDown;
        }
        else if (direction.Equals(Vector2.right))
        {
            Vector3 centerRight = new Vector3(neighborConnectPosition.x - extents.x, neighborConnectPosition.y, neighborConnectPosition.z);
            return centerRight;
        }
        throw new System.IndexOutOfRangeException("The direction is not supported");
    }

    public bool IsADirectionAvailable()
    {
        return IsNeighborAvailable(Vector2.up)
            || IsNeighborAvailable(Vector2.right)
            || IsNeighborAvailable(Vector2.left)
            || IsNeighborAvailable(Vector2.down);
    }

    public Vector2 GetRandomAvailableDirection()
    {
        ushort nbrAdded = 0;
        Vector2[] availableDirections = new Vector2[MAX_NBR_NEIGHBORS];
        if (IsNeighborAvailable(Vector2.up))
            availableDirections[nbrAdded++] = Vector2.up;
        if (IsNeighborAvailable(Vector2.left))
            availableDirections[nbrAdded++] = Vector2.left;
        if (IsNeighborAvailable(Vector2.down))
            availableDirections[nbrAdded++] = Vector2.down;
        if (IsNeighborAvailable(Vector2.right))
            availableDirections[nbrAdded++] = Vector2.right;

        if (nbrAdded == 0)
            return Vector2.zero;

        return availableDirections[Random.Range(0, nbrAdded - 1)];
    }

    public bool IsNeighborAvailable(Vector2 direction)
    {
        if (type == WallType.STRAIGHT || type == WallType.INVISIBLE) // Only 2 corners in the same direction
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
            return neighbor == null;
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogWarning(e.Message);
        }

        return false;
    }

    private static Vector2 RemoveNegation(Vector2 other)
    {
        return new Vector2(AbsoluteValue(other.x), AbsoluteValue(other.y));
    }

    private static float AbsoluteValue(float value)
    {
        if (value < 0)
            return value * -1;
        return value;
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
    private static ushort GetNeighborIndex(Vector2 direction)
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
        throw new System.IndexOutOfRangeException("The direction " + direction.ToString() + " is not supported");
    }

    // does not work for some odd reason...
    bool CheckForBlockages(Wall startingWall, Wall lastWall)
    {
        if (this.isInvisible || this.gameObject == lastWall.gameObject)
            return false;
        if (this.visitedWall == startingWall)
            return true;

        this.visitedWall = startingWall;

        for (ushort i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (this.neighbors[i] == null)
                continue;
            if (this.neighbors[i].CheckForBlockages(startingWall, this))
                return true;
        }
        return false;
    }

    public void MakeInvisibleIfBlocage()
    {
        if (isInvisible)
            return;
        for (ushort i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (this.neighbors[i] == null)
                continue;
            if (this.neighbors[i].CheckForBlockages(this, this))
            {
                MakeInvisible();
                UpdateInvisibilityForNeighbors();
                break;
            }
        }
    }

    private void UpdateInvisibilityForNeighbors()
    {
        for (ushort i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (this.neighbors[i] == null)
                continue;
            this.neighbors[i].MakeInvisibleIfNoVisibleNeighbor();
        }
    }

    void MakeInvisibleIfNoVisibleNeighbor()
    {
        if (type == WallType.INVISIBLE)
            return;

        for (var i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (neighbors[i] != null && !neighbors[i].GetComponent<Wall>().isInvisible)
            {
                MakeVisible();
                return;
            }
        }
        MakeInvisible();
    }

    void MakeInvisible()
    {
        if (isInvisible)
            return;
        isInvisible = true;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.layer = 6;
    }

    void MakeVisible()
    {
        if (!isInvisible || type == WallType.INVISIBLE)
            return;
        isInvisible = false;
        this.gameObject.layer = 0;
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
    }

    // Destroyed actions
    public void PrepareForDelete()
    {
        for (var i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (neighbors[i] != null)
                neighbors[i].NeighborDestroyed(this);
        }
    }

    void NeighborDestroyed(Wall other)
    {
        for (var i = 0; i < MAX_NBR_NEIGHBORS; i++)
        {
            if (neighbors[i] != null && neighbors[i] == other)
                neighbors[i] = null;
        }
    }
}
