using UnityEngine;

public class Jewel : MonoBehaviour, IHitable, IRecyclable
{
    [SerializeField] int RotationSpeed = 100;
    [SerializeField] int baseScore;
    [SerializeField] GameObject pointsFxPrefab;

    public ColorType Type { get; private set; }
    JewelManager jewelManager;

    public void Initialize(ColorType type, JewelManager jewelManager)
    {
        Type = type;
        this.jewelManager = jewelManager;

        if (GetComponent<SphereCollider>() == null)
        {
            var collider = gameObject.AddComponent<SphereCollider>();
            collider.center = new Vector3(0, 0, 0);
            collider.radius = 1;
        }
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.unscaledDeltaTime);
    }

    public void HandleHit(ColorType selectedType)
    {
        if (selectedType == Type) HandleCorrectHit();
    }

    public void HandleCorrectHit()
    {
        jewelManager.HandleJewelHit(this);
    }

    public void Recycle()
    {
        jewelManager.ObjectsPool.Recycle(gameObject);
    }

    public int GetScore()
    {
        return baseScore;
    }
}