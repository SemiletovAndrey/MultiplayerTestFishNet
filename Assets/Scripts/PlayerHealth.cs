using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private readonly SyncVar<int> health = new SyncVar<int>(10);
    public TextMeshPro text;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            GetComponent<PlayerHealth>().enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateHealthServer(this, -1);
        }
    }

    [ServerRpc]
    public void UpdateHealthServer(PlayerHealth script, int amountToChange)
    {
        health.Value += amountToChange;
        UpdateHealth(script, health.Value);
        Debug.Log(health.Value);
    }

    [ObserversRpc]
    public void UpdateHealth(PlayerHealth script, int health)
    {
        //script.health = health;
        script.text.text = health.ToString();

    }
}
