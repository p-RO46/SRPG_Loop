using System.Collections;
using UnityEngine;

public class MB_SRPG_ButtonEvents : MonoBehaviour
{
    private MB_PlayerManager playerManager;
    private Coroutine buttonEventCoroutine = null;
    private bool okPressed = false;

    void Start()
    {
        playerManager = GetComponent<MB_PlayerManager>();
    }

    public void OnAttackClicked()
    {
        if(buttonEventCoroutine != null)
        {
            return;
        }
        buttonEventCoroutine = StartCoroutine(AttackCoroutine());
        playerManager.ReadySkill();
    }
    private IEnumerator AttackCoroutine()
    {
        while (!okPressed)
        {
            yield return null;
        }
        playerManager.StartSkill();
        buttonEventCoroutine = null;
        okPressed = false;
    }

    public void OnMoveClicked()
    {
        if (buttonEventCoroutine != null)
        {
            return;
        }
        buttonEventCoroutine = StartCoroutine(MoveCoroutine());
        playerManager.ReadyMove();
    }
    private IEnumerator MoveCoroutine()
    {
        while (!okPressed)
        {
            yield return null;
        }
        playerManager.StartMove();
        buttonEventCoroutine = null;
        okPressed = false;
    }

    public void OnSwapClassClicked()
    {
        if (buttonEventCoroutine != null)
        {
            return;
        }
        buttonEventCoroutine = StartCoroutine(SwapClassCoroutine());
        playerManager.ReadySwapClass();
    }
    private IEnumerator SwapClassCoroutine()
    {
        while (!okPressed)
        {
            yield return null;
        }
        playerManager.StartSwapClass();
        buttonEventCoroutine = null;
        okPressed = false;
    }

    public void OnOkClikced()
    {
        if (buttonEventCoroutine == null)
        {
            return;
        }
        okPressed = true;
    }
}
