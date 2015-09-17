using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Engine/AdvancedMotor")]
public class AdvancedMotor : Motor {

    public enum MovementMode {
        walking,
        running,
        sneaking,
    }

    public MovementMode curMovementMode;

    public float walkSpeed = 1.5f;
    public float runSpeed = 2.0f;
    /*
    public float runSpeed {
        get { return maxDesiredDirecMag; }
        set { maxDesiredDirecMag = value; }
    }
    */    public float sneakSpeed = 1.0f;

    /*
    [Range(0.0f, 1.0f)]
    public float backwardsMovementModifier = 0.3f;
    [Range(0.0f, 1.0f)]
    public float sidewaysMovementModifier = 0.5f;
    */

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
    
    protected override void HandleMovement() {
        
        //befoe calculating the trueDirec to move the agent in we
         //modify the magnitude of the desired direction according to the movement state
         //which will modify the point we want the trueDirec to move towards,
         //thereby modifying the move speed
        
    //WALK
        if (curMovementMode == MovementMode.walking) {
            desiredDirec *= walkSpeed;
        }
    //RUN
        else if (curMovementMode == MovementMode.running) {
            desiredDirec *= runSpeed;
        }
    //SNEAK
        else if (curMovementMode == MovementMode.sneaking) {
            desiredDirec *= sneakSpeed;
        }

        //call the regular function to modify the trueDirec
        base.HandleMovement();

        
        


    }


}
