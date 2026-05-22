using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ChargeDragShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Camera cam;

    [Header("Physics")]
    [SerializeField] private bool use2D = true;
    [SerializeField] private float maxChargeTime = 2.0f;
    [SerializeField] private float maxDragDistance = 4.0f;
    [SerializeField] private float maxForce = 20f;
    [SerializeField] private float minForce = 2f;
    [SerializeField] private ForceMode2D forceMode2D = ForceMode2D.Impulse;
    [SerializeField] private ForceMode forceMode3D = ForceMode.Impulse;

    [Header("Line")]
    [SerializeField] private float lineMaxLength = 4.0f;
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private Gradient lineGradient; // opzionale: colore in base alla potenza

    private LineRenderer lr;
    private bool isCharging;
    private float chargeTimer;
    private Vector3 dragVectorWorld;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        if (lineGradient.colorKeys != null && lineGradient.colorKeys.Length > 0)
            lr.colorGradient = lineGradient;
        lr.enabled = false; // nascosta all’avvio
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            chargeTimer = 0f;
            lr.enabled = true;
        }

        if (isCharging && Input.GetMouseButton(0))
        {
            chargeTimer += Time.deltaTime;
            Vector3 mouseWorld = GetMouseWorld();
            Vector3 origin = firePoint ? firePoint.position : transform.position;

            // drag dal punto di fuoco verso il mouse
            dragVectorWorld = (mouseWorld - origin);

            // Direzione di mira = opposta al drag
            Vector3 aimDir = -dragVectorWorld;
            if (use2D) aimDir.z = 0f;

            float dragLen = Mathf.Min(aimDir.magnitude, Mathf.Max(maxDragDistance, 0.0001f));
            float t = Mathf.Clamp01(dragLen / maxDragDistance);
            float lineLen = Mathf.Lerp(0.3f, lineMaxLength, t * 1.0f);

            Vector3 endPos = origin + (aimDir.sqrMagnitude > 0.0001f ? aimDir.normalized * lineLen : Vector3.right * 0.3f);

            // Aggiorna i due punti della linea
            lr.SetPosition(0, origin);
            lr.SetPosition(1, endPos);
        }

        if (isCharging && Input.GetMouseButtonUp(0))
        {
            FireProjectile();
            isCharging = false;
            lr.enabled = false;
            dragVectorWorld = Vector3.zero;
            chargeTimer = 0f;
        }
    }

    private Vector3 GetMouseWorld()
    {
        Vector3 mouse = Input.mousePosition;
        if (use2D)
        {
            Vector3 w = cam.ScreenToWorldPoint(mouse);
            w.z = 0f;
            return w;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, (firePoint ? firePoint.position : transform.position));
            Ray ray = cam.ScreenPointToRay(mouse);
            if (plane.Raycast(ray, out float enter)) return ray.GetPoint(enter);
            Vector3 w = cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 10f));
            return w;
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Direzione opposta al drag
        Vector3 dir = -dragVectorWorld;
        if (use2D) dir.z = 0f;

        float dragMag = dir.magnitude;
        if (dragMag > 0.0001f) dir /= dragMag;
        else dir = use2D ? Vector3.right : Vector3.forward;

        float chargeFactor = Mathf.Clamp01(chargeTimer / maxChargeTime);
        float dragFactor = Mathf.Clamp01(dragMag / maxDragDistance);
        float force = Mathf.Lerp(minForce, maxForce, Mathf.Clamp01(0.5f * chargeFactor + 0.5f * dragFactor));

        Vector3 spawnPos = firePoint ? firePoint.position : transform.position;
        Quaternion spawnRot = Quaternion.LookRotation(use2D ? Vector3.forward : dir, Vector3.up);
		GameObject go = Instantiate(projectilePrefab, spawnPos, spawnRot);
		go.GetComponent<DoDamage>()?.SetShooter(this.gameObject);

        if (use2D)
        {
            var rb2d = go.GetComponent<Rigidbody2D>();
            if (rb2d)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0f;
                rb2d.AddForce((Vector2)dir * force, forceMode2D);
            }
        }
        else
        {
            var rb = go.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(dir * force, forceMode3D);
            }
        }
    }
}
