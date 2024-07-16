using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class CubePositionBroadcast : MonoBehaviour
{
    public List<Transform> cubePositions = new List<Transform>();
    public int transformIndex;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<PositionIndex>(OnPositionBroadcast);
        InstanceFinder.ServerManager.RegisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<PositionIndex>(OnPositionBroadcast);
        InstanceFinder.ServerManager.UnregisterBroadcast<PositionIndex>(OnClientPositionBroadcast);
    }



    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            int nextIndex = transformIndex + 1;
            if (nextIndex >= cubePositions.Count)
            {
                nextIndex = 0;
            }
            if (InstanceFinder.IsServer)
            {
                InstanceFinder.ServerManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
            else if (InstanceFinder.IsClient)
            {
                InstanceFinder.ClientManager.Broadcast(new PositionIndex() { tIndex = nextIndex });
            }
        }
    }

    private void OnClientPositionBroadcast(NetworkConnection networkConnection, PositionIndex positionIndex, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(positionIndex);
    }

    private void OnPositionBroadcast(PositionIndex index, Channel channel)
    {
        transformIndex = index.tIndex;
        UpdateCubePosition();
    }

    private void UpdateCubePosition()
    {
        transform.position = cubePositions[transformIndex].position;
    }

    public struct PositionIndex : IBroadcast
    {
        public int tIndex;
    }
}
