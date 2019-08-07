using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T>
{

    List<T> activeObstacles = new List<T>();
    List<T> inactiveObstacles = new List<T>();

    public bool readyToRecycle;

    private int activeListLength, inactiveListLength;

    public ObjectPooler(int activeListLength, int inactiveListLength)
    {
        this.activeListLength = activeListLength;
        this.inactiveListLength = inactiveListLength;
    }

    public void AddObstacleToActiveList(T obstacle)
    {
        activeObstacles.Add(obstacle);
        if(activeObstacles.Count > activeListLength)
        {
            AddObstacleToInactiveList(activeObstacles[0]);
            activeObstacles.RemoveAt(0);
        }
    }

    void AddObstacleToInactiveList(T obstacle)
    {
        if(inactiveObstacles.Count > inactiveListLength)
        {
            readyToRecycle = true;
        }
        inactiveObstacles.Add(obstacle);
        if(typeof(T) == typeof(GameObject))
        {
            //GameObject obs = T; // T.SetActive(false);

        }
    }

    public T RequestInactiveObstacle()
    {
        int randomIndex = Random.Range(0, inactiveObstacles.Count);
        T randomObstacle = inactiveObstacles[randomIndex];
        RemoveFromListAfter(randomIndex);
        return randomObstacle;
    }

    void RemoveFromListAfter(int index)
    {
        inactiveObstacles.RemoveAt(index);
    }
}
