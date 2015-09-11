using UnityEngine;
using System.Collections;

public class AdvancedMotor : Motor {

    public float walkSpeed = 1.5f;
    public float runSpeed {
        get { return maxDesiredDirecMag; }
        set { maxDesiredDirecMag = value; }
    }
    public float sneakSpeed = 1.0f;

    [Range(0.0f, 1.0f)]
    public float backwardsMovementModifier = 0.3f;
    [Range(0.0f, 1.0f)]
    public float sidewaysMovementModifier = 0.5f;


    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

}
