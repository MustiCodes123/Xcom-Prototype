using UnityEngine;
using Zenject;

public class BaseDecale : MonoBehaviour
{
    public DecaleType DecaleType;
    public DynamicJoystick Joystick;
    public Transform Target;
    public Transform LookAt;
    public float offsetY;

    [SerializeField] protected int maxRange;
    [SerializeField] protected float rotationDelta;

    protected BaseCharacerView _caster;
    protected void FixedUpdate()
    {
        DecaleUpdate();
    }

    public void Setup(DynamicJoystick Joystick, BaseCharacerView characterView)
    {
        this.Joystick = Joystick;
        _caster = characterView;
    }

    public void OnSpawned()
    {
        gameObject.SetActive(true);
    }

    public void OnDispawned()
    {
        gameObject.SetActive(false);
    }

    public virtual void DecaleUpdate()
    {
        if (Joystick != null)
        {
            float joysticX = Joystick.Horizontal;
            float joysticY = Joystick.Vertical;

            Quaternion cameraRotation = Camera.main.transform.rotation;
            Vector3 correctedDirection = cameraRotation * new Vector3(-joysticX, 0f, -joysticY);

            var angle = Mathf.Atan2(correctedDirection.x, correctedDirection.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, angle + offsetY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationDelta);

            transform.position = _caster.transform.position;

            _caster.LookAtTarget(Target);
        }
    }

    public class Factory : PlaceholderFactory<DecaleType, BaseDecale>
    {


    }
}
