using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] private List<Neightbour> neightbours;

    private void Awake()
    {
        var TwoWayNeighbours = neightbours.Where(n => n.isTwoWay);
        foreach(var neightbour in TwoWayNeighbours) 
        {
            neightbour.point?.CreateConnection(this, -neightbour.direction, neightbour.connectionType, neightbour.isTwoWay);
        }
    }

    public void CreateConnection(ClimbPoint point, Vector2 direction, ConnectionType connectionType,
        bool isTwoWay = true)
    {
        var neighbour = new Neightbour()
        {
            point = point,
            direction = direction,
            connectionType = connectionType,
            isTwoWay = isTwoWay
        };

        neightbours.Add(neighbour);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        foreach (var neighbour in neightbours)
        {
            if (neighbour.point != null)
                Debug.DrawLine(transform.position, neighbour.point.transform.position, (neighbour.isTwoWay) ? Color.green : Color.gray);
        }
    }
}

[System.Serializable]
public class Neightbour
{
    public ClimbPoint point;
    public Vector2 direction;
    public ConnectionType connectionType;
    public bool isTwoWay = true;
}

public enum ConnectionType { Jump, move}
