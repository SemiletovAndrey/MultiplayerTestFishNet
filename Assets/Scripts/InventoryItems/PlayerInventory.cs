using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;
using TMPro;
using UnityEngine.UI;

public class PlayerInventory : NetworkBehaviour
{
    [Header("Inventory settings")]
    public List<InventoryObject> inventoryObjects = new List<InventoryObject>();
    private GameObject _invPanel;
    private Transform _invObjectHolder;
    [SerializeField] private GameObject invCanvasObject;
    [SerializeField] private KeyCode inventoryButton = KeyCode.Tab;

    [Header("Pickup settings")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupDistance;
    [SerializeField] private KeyCode pickupButton = KeyCode.E;

    private Camera _camera;
    private Transform _worldObjectHolder;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            enabled = false;
            return;
        }

        _camera = Camera.main;
        _worldObjectHolder = GameObject.FindGameObjectWithTag("WorldObjects").transform;
        _invPanel = GameObject.FindGameObjectWithTag("InventoryPanel");
        _invObjectHolder = GameObject.FindGameObjectWithTag("InventoryObjectHolder").transform;
        if (_invPanel.activeSelf)
            ToggleInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupButton))
        {
            Pickup();
        }
        if (Input.GetKeyDown(inventoryButton))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (_invPanel.activeSelf)
        {
            _invPanel.SetActive(false);
        }
        else if (!_invPanel.activeSelf)
        {
            UpdateUI();
            _invPanel.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        foreach(Transform child in _invObjectHolder)
            Destroy(child.gameObject);

        foreach (var invObj in inventoryObjects)
        {
            GameObject obj = Instantiate(invCanvasObject, _invObjectHolder);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{invObj.Item.itemName} - {invObj.amount}";
            obj.GetComponent<Button>().onClick.AddListener(delegate { DropItem(invObj.Item); });
        }
    }

    private void DropItem(ItemObject item)
    {
        foreach (var invObj in inventoryObjects)
        {
            if (invObj.Item != item)
                continue;
            if (invObj.amount > 1)
            {
                invObj.amount--;
                DropItemRPC(invObj.Item.prefab, _camera.transform.position + _camera.transform.forward);
                UpdateUI();
                return;
            }
            if (invObj.amount <= 1)
            {
                inventoryObjects.Remove(invObj);
                DropItemRPC(invObj.Item.prefab, _camera.transform.position + _camera.transform.forward);
                UpdateUI();
                return;
            }
        }
    }

    private void Pickup()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, pickupDistance, pickupLayer))
        {
            if (!hit.transform.TryGetComponent<GroundItem>(out GroundItem groundItem))
                return;

            AddToInventory(groundItem.itemObjectScriptable);
            Despawn(groundItem.gameObject);
        }
    }

    private void AddToInventory(ItemObject itemObject)
    {
        foreach (InventoryObject invObj in inventoryObjects)
        {
            if (invObj.Item == itemObject)
            {
                invObj.amount++;
                return;
            }
        }
        inventoryObjects.Add(new InventoryObject() { Item = itemObject, amount = 1 });
    }

    [ServerRpc(RequireOwnership = false)]
    private void Despawn(GameObject objToDesapwn)
    {
        ServerManager.Despawn(objToDesapwn, DespawnType.Destroy);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemRPC(GameObject prefab, Vector3 position)
    {
        GameObject drop = Instantiate(prefab, position, Quaternion.identity, _worldObjectHolder);
        ServerManager.Spawn(drop);
    }

    [Serializable]
    public class InventoryObject
    {
        public ItemObject Item;
        public int amount;
    }
}
