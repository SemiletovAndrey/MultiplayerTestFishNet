using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    public readonly SyncVar<int> health = new SyncVar<int>(10);
    public TextMeshPro text;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            GetComponent<PlayerHealth>().enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServer( int amountToChange)
    {
        health.Value += amountToChange;
        UpdateHealth(health.Value);
        Debug.Log(health.Value);
    }

    [ObserversRpc]
    public void UpdateHealth( int health)
    {
        text.text = health.ToString();
    }

    public void TakeDamage(int damage)
    {
        if(damage > 0)
        {
            throw new Exception("Invalid damage");
        }
        UpdateHealthServer(damage);
    }
}
