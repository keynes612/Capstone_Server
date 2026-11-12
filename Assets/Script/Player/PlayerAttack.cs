using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System 네임스페이스 추가

public class PlayerAttack : NetworkBehaviour
{
    [Header("Attack Area Object")]
    [SerializeField] private GameObject attackAreaObject; // 자식 attack_area 할당
    [SerializeField] private float activeDuration = 0.2f;  // 활성화 유지 시간
    [SerializeField] private float cooldownTime = 0.5f;    // 쿨타임

    private bool isCoolingDown = false;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        if (attackAreaObject != null)
        {
            attackAreaObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Mouse.current 체크 및 좌클릭 프레임 감지
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isCoolingDown)
        {
            ToggleAttackAreaServerRpc();
        }
    }

    [ServerRpc]
    private void ToggleAttackAreaServerRpc()
    {
        ToggleAttackAreaClientRpc();
    }

    [ClientRpc]
    private void ToggleAttackAreaClientRpc()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(ActivateAreaRoutine());
    }

    private IEnumerator ActivateAreaRoutine()
    {
        isCoolingDown = true;

        if (attackAreaObject != null)
        {
            attackAreaObject.SetActive(true);
        }

        yield return new WaitForSeconds(activeDuration);

        if (attackAreaObject != null)
        {
            attackAreaObject.SetActive(false);
        }

        float remainingCooldown = cooldownTime - activeDuration;
        if (remainingCooldown > 0f)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }

        isCoolingDown = false;
    }
}