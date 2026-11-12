using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System 추가

public class NetworkPlayer : NetworkBehaviour
{

    // num 객체 생성, 해당 객체에 
    private NetworkVariable<int> num = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        num.OnValueChanged += OnNumChanged;
    }

    public override void OnNetworkDespawn()
    {
        num.OnValueChanged -= OnNumChanged;
    }

    private void OnNumChanged(int previousValue, int newValue)
    {
        Debug.Log($"OwnerClientId: {OwnerClientId}, num: {newValue}");
    }

    private void Update()
    {
        if (!IsOwner) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            num.Value = Random.Range(0, 100);
        }
    }
}