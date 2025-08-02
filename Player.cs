using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public PlayerMouvement pMouvement { get; private set; }
    [SerializeField] public PlayerInventory pInventory { get; private set; }

    [SerializeField] public PlayerCamera pCamera { get; private set; }

    void Awake()
    {
        pMouvement = this.GetComponent<PlayerMouvement>();
        pInventory = this.GetComponent<PlayerInventory>();
        pCamera = this.GetComponent<PlayerCamera>();
    }

    void Start()
    {
        pInventory.Init(this);
        pMouvement.Init(this);
        pCamera.Init(this);
    }
}
