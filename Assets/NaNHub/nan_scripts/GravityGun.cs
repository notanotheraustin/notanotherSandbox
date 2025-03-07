using UnityEngine;

public class GravityGun : MonoBehaviour
{
    public Transform holdPoint;
    public float grabRange = 10f;
    public float throwForce = 20f;
    public float pullForce = 10f;
    public float rotationSpeed = 100f;
    public ParticleSystem pullEffect;
    public Material highlightMaterial;
    private MaterialPropertyBlock propertyBlock;
    private Rigidbody heldObject;
    private Rigidbody pullingObject;
    public bool debugRaycasts = false;
    private RaycastHit lastHit;
    private bool hitSomething = false;
    private Renderer objectRenderer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (heldObject == null)
                TryGrabObject();
            else
                ThrowObject();
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            TryPullObject();
        }
        else if (Input.GetMouseButtonUp(1)) 
        {
            pullingObject = null;
            if (pullEffect != null && pullEffect.isPlaying)
                pullEffect.Stop();
        }

        if (heldObject != null)
        {
            RotateHeldObject();
        }
    }

    void FixedUpdate()
    {
        if (pullingObject != null)
        {
            Vector3 direction = (holdPoint.position - pullingObject.position).normalized;
            pullingObject.AddForce(direction * pullForce, ForceMode.Acceleration);
            
            if (pullEffect != null && !pullEffect.isPlaying)
                pullEffect.Play();
        }
    }

    void TryGrabObject()
    {
        if (Physics.Raycast(holdPoint.position, Camera.main.transform.forward, out lastHit, grabRange))
        {
            hitSomething = true;
            if (lastHit.rigidbody != null)
            {
                heldObject = lastHit.rigidbody;
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f;
                heldObject.transform.parent = holdPoint;
                
                ApplyHighlight(heldObject.gameObject);
                
                if (pullEffect != null && pullEffect.isPlaying)
                    pullEffect.Stop();
            }
        }
        else
        {
            hitSomething = false;
        }
    }

    void TryPullObject()
    {
        if (Physics.Raycast(holdPoint.position, Camera.main.transform.forward, out lastHit, grabRange))
        {
            hitSomething = true;
            if (lastHit.rigidbody != null && lastHit.rigidbody != heldObject)
            {
                pullingObject = lastHit.rigidbody; 
                
                if (pullEffect != null)
                    pullEffect.Play();
            }
        }
        else
        {
            hitSomething = false;
        }
    }

    void ThrowObject()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;
        heldObject.transform.parent = null;
        heldObject.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        
        RemoveHighlight(heldObject.gameObject);
        heldObject = null;
    }

    void RotateHeldObject()
    {
        float rotateX = Input.GetAxis("Mouse ScrollWheel") * rotationSpeed * Time.deltaTime;
        float rotateY = 0f;
        
        if (Input.GetKey(KeyCode.Q)) rotateY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) rotateY = rotationSpeed * Time.deltaTime;
        
        heldObject.transform.Rotate(holdPoint.transform.up, rotateY, Space.World);
        heldObject.transform.Rotate(holdPoint.transform.right, rotateX, Space.World);
    }

    void ApplyHighlight(GameObject obj)
    {
        objectRenderer = obj.GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
                
            objectRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", Color.yellow * 2f); 
            objectRenderer.SetPropertyBlock(propertyBlock);
            
            objectRenderer.material.EnableKeyword("_EMISSION"); 
        }
    }

    void RemoveHighlight(GameObject obj)
    {
        if (objectRenderer != null)
        {
            propertyBlock.SetColor("_EmissionColor", Color.black);
            objectRenderer.SetPropertyBlock(propertyBlock);
            objectRenderer.material.DisableKeyword("_EMISSION"); 
            objectRenderer = null;
        }
    }

    void OnDrawGizmos()
    {
        if (debugRaycasts)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(holdPoint.position, Camera.main.transform.forward * grabRange);
            
            if (hitSomething)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(lastHit.point, 0.2f);
            }
        }
    }
}
