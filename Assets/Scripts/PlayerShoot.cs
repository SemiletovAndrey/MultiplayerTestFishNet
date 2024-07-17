using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerShoot : NetworkBehaviour
{
    public int damage = 1;

    public float timeBetweenFire;

    private float _fireTimer;

    private void Update()
    {
        if(!base.IsOwner) return;

        if (Input.GetButton("Fire1"))
        {
            if(_fireTimer <= 0)
            {
                ShotServer(damage, Camera.main.transform.position, Camera.main.transform.forward);
                _fireTimer = timeBetweenFire;
            }
        }

        if(_fireTimer > 0)
        {
            _fireTimer -= Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShotServer(int damageToGive,Vector3 position, Vector3 direction)
    {
        if (Physics.Raycast(position, direction, out RaycastHit hit) && hit.transform.TryGetComponent(out PlayerHealth enemyHealth))
        {
            enemyHealth.TakeDamage(-damageToGive);
        }
    }
}
