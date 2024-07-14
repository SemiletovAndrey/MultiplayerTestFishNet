using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerSpawnObject : NetworkBehaviour
{
    public GameObject objToSpawn;
    private GameObject spawnedObject;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<PlayerSpawnObject>().enabled = false;
        }
    }

    private void Update()
    {
        if(spawnedObject == null && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(objToSpawn, transform, this);
        }
        if(spawnedObject != null && Input.GetKeyDown(KeyCode.Alpha2))
        {
            DespawnObject(spawnedObject);
        }
    }

    [ServerRpc]
    public void SpawnObject(GameObject obj, Transform playerTransform, PlayerSpawnObject script)
    {
        GameObject spawned = Instantiate(obj, playerTransform.position + playerTransform.forward, Quaternion.identity);
        ServerManager.Spawn(spawned);
        SetSpawnedObject(spawned, script);

    }

    [ObserversRpc]
    public void SetSpawnedObject(GameObject spawned, PlayerSpawnObject script)
    {
        script.spawnedObject = spawned;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }

}
