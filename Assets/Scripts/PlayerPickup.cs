using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;

public class PlayerPickup : NetworkBehaviour
{
    [SerializeField] private KeyCode pickupButton = KeyCode.E;
    [SerializeField] private KeyCode dropButton = KeyCode.Q;
    [SerializeField] private float raycastDistance = 1;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private Transform pickupPosition;

    private Camera _camera;
    private bool _hasObjectInHand;
    private GameObject _objectInHand;
    private Transform _worldObjectHolder;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            GetComponent<PlayerPickup>().enabled = false;

        _camera = Camera.main;
        _worldObjectHolder = GameObject.FindGameObjectWithTag("WorldObjects").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupButton))
        {
            Pickup();
        }
        if (Input.GetKeyDown(dropButton))
        {
            Drop();
        }
    }

    

    private void Pickup()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, raycastDistance, pickupLayer))
        {
            if (!_hasObjectInHand)
            {
                SetObjectInHandServer(hit.transform.gameObject, pickupPosition.position, pickupPosition.rotation, gameObject);
                _objectInHand = hit.transform.gameObject;
                _hasObjectInHand = true;
            }
            else if (_hasObjectInHand)
            {
                Drop();
                SetObjectInHandServer(hit.transform.gameObject, pickupPosition.position, pickupPosition.rotation, gameObject);
                _objectInHand = hit.transform.gameObject;
                _hasObjectInHand = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetObjectInHandServer(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
    {
        SetObjectInHandObserver(obj, position, rotation, player);
    }

    [ObserversRpc]
    public void SetObjectInHandObserver(GameObject obj, Vector3 position, Quaternion rotation, GameObject player)
    {
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.parent = player.transform;

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
            obj.GetComponent<Collider>().enabled = false;
        }
    }


    private void Drop()
    {
        if (!_hasObjectInHand)
            return;

        DropObjectInHandServer(_objectInHand, _worldObjectHolder);
        _hasObjectInHand = false;
        _objectInHand = null;

    }

    [ServerRpc(RequireOwnership = false)]
    public void DropObjectInHandServer(GameObject obj, Transform worldObjects)
    {
        DropObjectInHandObserver(obj, worldObjects);
    }

    [ObserversRpc]
    private void DropObjectInHandObserver(GameObject obj, Transform worldObjects)
    {
        obj.transform.parent = worldObjects;

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
            obj.GetComponent<Collider>().enabled = true;
        }
    }
}
