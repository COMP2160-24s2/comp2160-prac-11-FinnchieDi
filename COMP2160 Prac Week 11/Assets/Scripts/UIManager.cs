/**
 * A singleton class to allow point-and-click movement of the marble.
 * 
 * It publishes a TargetSelected event which is invoked whenever a new target is selected.
 * 
 * Author: Malcolm Ryan
 * Version: 1.0
 * For Unity Version: 2022.3
 */

using UnityEngine;
using UnityEngine.InputSystem;

// note this has to run earlier than other classes which subscribe to the TargetSelected event
[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
    [SerializeField] private bool useDeltaMovement = true;
#region UI Elements
    [SerializeField] private Transform crosshair;
    [SerializeField] private Transform target;
    [SerializeField] private Camera mainCam;
    private Plane plane;
#endregion 

#region Singleton
    static private UIManager instance;
    static public UIManager Instance
    {
        get { return instance; }
    }
#endregion 

#region Actions
    private Actions actions;
    private InputAction mouseAction;
    private InputAction deltaAction;
    private InputAction selectAction;
    private InputAction mouseZoomAction;
#endregion

#region Events
    public delegate void TargetSelectedEventHandler(Vector3 worldPosition);
    public event TargetSelectedEventHandler TargetSelected;
#endregion

#region Init & Destroy
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one UIManager in the scene.");
        }

        instance = this;

        actions = new Actions();
        mouseAction = actions.mouse.position;
        deltaAction = actions.mouse.delta;
        selectAction = actions.mouse.select;
        mouseZoomAction = actions.camera.zoom;

        Cursor.visible = false;
        target.gameObject.SetActive(false);
        mouseZoomAction.performed += CameraZoom;
    }

    void OnEnable()
    {
        actions.mouse.Enable();
        actions.camera.Enable();
    }

    void OnDisable()
    {
        actions.mouse.Disable();
        actions.camera.Disable();
    }
#endregion Init

    void Start()
    {
        plane = new Plane(Vector3.up, transform.position);
    }

#region Update
    void Update()
    {
        MoveCrosshair();
        SelectTarget();
    }

    private void MoveCrosshair() 
    {
        if (useDeltaMovement)
        {
            private Vector3 crosshairWorldPos = crosshair.transform.position;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            Vector3 screenPos = mainCam.WorldtoScreenPoint(crosshairWorldPos);
            screenPos += (Vector3)mouseDelta;

            Ray ray = mainCam.ScreenPointToRay(screenPos);
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                crosshairWorldPos = hitPoint;

                crosshair.transform.position = crosshairWorldPos;
                crosshair.transform.localPosition = new Vector3(crosshair.transform.localPosition.x, crosshair.transform.localPosition.y, 0);
            }
        }
        else
        {
            Vector2 mousePos = mouseAction.ReadValue<Vector2>();
            //Debug.Log("Mouse Position = " + mousePos);
            mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        

            Ray aim = mainCam.ScreenPointToRay(mousePos);
            float enter = 0.0f;
        

            if (plane.Raycast(aim, out enter))
            {
            
                Vector3 hitPoint = aim.GetPoint(enter);
                Debug.Log(enter); 
                //Debug.Log("aim x = " + hitPoint.x + " aim y = " + hitPoint.y);
                crosshair.transform.position = hitPoint;
                hitPoint = crosshair.transform.localPosition;
                hitPoint.z = 0;
                crosshair.transform.localPosition = hitPoint;
            }
        }

    }

    private void SelectTarget()
    {
        if (selectAction.WasPerformedThisFrame())
        {
            // set the target position and invoke 
            target.gameObject.SetActive(true);
            target.position = crosshair.position;     
            TargetSelected?.Invoke(target.position);       
        }
    }

    private void CameraZoom(InputAction.CallbackContext context)
    {
        float zoomVal = mouseZoomAction.ReadValue<float>();

        Debug.Log(zoomVal);
        
        mainCam.fieldOfView -= zoomVal/12; 
    }

#endregion Update

}
