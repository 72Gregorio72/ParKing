using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalScript : MonoBehaviour
{
    public PortalScript otherPortal;
    public bool canTeleport = true; // consente 1 teleport finché non si esce

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canTeleport || otherPortal == null) return;

        // Teletrasporta
       other.transform.position = otherPortal.transform.position;

        // Blocca questo portale finché non si esce
        canTeleport = false;
        // Blocca anche il portale di arrivo per evitare rimbalzo
        otherPortal.canTeleport = false;
        // Sbloccheremo entrambi su OnTriggerExit2D del relativo portale
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Quando si esce dal volume, si può teletrasportare di nuovo
        canTeleport = true;
    }
}
