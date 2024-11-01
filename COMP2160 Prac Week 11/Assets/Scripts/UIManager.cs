using UnityEngine;
using UnityEngine.InputSystem;

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
    #endregion

    void Start() 
    {
        plane = new Plane(transform.up, transform.position);
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
            Vector2 mousePos = mouseAction.ReadValue<Vector2>();
                      
            Vector2 mouseDelta = deltaAction.ReadValue<Vector2>();
            Debug.Log("mouseDelta = " + mouseDelta);

            Vector2 screenPos = mainCam.WorldToScreenPoint(mousePos);
            Debug.Log("screenPos = " + screenPos);

            screenPos = new Vector3(screenPos.x + mouseDelta.x, screenPos.y + mouseDelta.y, 0);
            Debug.Log("NEW screenPos = " + screenPos);

            // screenPos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            // screenPos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);  
            
            Ray ray = mainCam.ScreenPointToRay(screenPos);
            float enter = 0.0f;
            if (plane.Raycast(ray, out enter)) 
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                crosshair.transform.position = hitPoint;
                hitPoint = crosshair.transform.localPosition;
                hitPoint.z = 0;
                crosshair.transform.localPosition = hitPoint;
            }
        } 
        else 
        {
            // Vector2 mousePos = mouseAction.ReadValue<Vector2>();
            // mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
            // mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
            // Ray aim = mainCam.ScreenPointToRay(mousePos);
            // float enter = 0.0f;
            // if (plane.Raycast(aim, out enter)) 
            // {
            //     Vector3 hitPoint = aim.GetPoint(enter);
            //     Debug.Log(enter);
            //     crosshair.transform.position = hitPoint;
            //     hitPoint = crosshair.transform.localPosition;
            //     hitPoint.z = 0;
            //     crosshair.transform.localPosition = hitPoint;
            // }
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
        mainCam.fieldOfView -= zoomVal / 12;
    }
    #endregion
}