using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private Transform ball;
    private Vector3 startMousePos, startBallPos;
    private bool moveTheBall,gameState,DetectNewPath;
    [Range(0f,1f)] public float maxSpeed;
    [Range(0f,1f)] public float camSpeed;
    [Range(0f, 50f)] public float pathSpeed;
    [Range(0f, 1000f)] public float ballRotateSpeed;
    private float velocity, camVelocity_x,camVelocity_y;
    private Camera mainCam;
    public Transform path;
    private Rigidbody rb;
    private Collider _collider;
    private Renderer BallRenderer;
    public ParticleSystem CollideParticle;
    public ParticleSystem airEffect;
    public ParticleSystem Dust;
    public ParticleSystem BallTrail;
    public Material[] Ballmats = new Material[2];
    void Start()
    {
        ball = transform;
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        BallRenderer = ball.GetChild(1).GetComponent<Renderer>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && MenuManager.MenuManagerInstance.GameState)
        {
            moveTheBall = true;
            BallTrail.Play();
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray,out var distance))
            {
                startMousePos = ray.GetPoint(distance);
                startBallPos = ball.position;
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }

        if (moveTheBall)
        {
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray, out var distance))
            {
                Vector3 mouseNewPos = ray.GetPoint(distance);
                Vector3 MouseNewPos = mouseNewPos - startMousePos;
                Vector3 DesireBallPos = MouseNewPos + startBallPos;

                DesireBallPos.x = Mathf.Clamp(DesireBallPos.x, -1.5f, 1.5f);
                            
                ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x,DesireBallPos.x,ref velocity , maxSpeed)
                    , ball.position.y,ball.position.z);
                            
            }
        }

        if (MenuManager.MenuManagerInstance.GameState)
        {
            var pathPosition = path.position;
            path.position = Vector3.MoveTowards(pathPosition,new Vector3(pathPosition.x,pathPosition.y,-1000f), Time.deltaTime * pathSpeed);
            ball.GetChild(1).Rotate(Vector3.right * ballRotateSpeed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        var cameraNewPos = mainCam.transform.position;
        
        if (rb.isKinematic)
            mainCam.transform.position = new Vector3(Mathf.SmoothDamp(cameraNewPos.x,ball.transform.position.x,ref camVelocity_x,camSpeed )
                , Mathf.SmoothDamp(cameraNewPos.y,ball.transform.position.y + 3f,ref camVelocity_y,camSpeed ),cameraNewPos.z);
  
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstacle"))
        {
            gameObject.SetActive(false);
            MenuManager.MenuManagerInstance.GameState = false;
            MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<Text>().text = "You Lose";

        }

        switch (other.tag)
        {
            case "red":
                other.gameObject.SetActive(false);
                Ballmats[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = Ballmats;
                var NewParticle = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var BallTrailColor = BallTrail.trails;
                BallTrailColor.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            
            case "green":
                other.gameObject.SetActive(false);
                Ballmats[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = Ballmats;
                var NewParticle1 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle1.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var BallTrailColor_1 = BallTrail.trails;
                BallTrailColor_1.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            
            case "yellow":
                other.gameObject.SetActive(false);
                Ballmats[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = Ballmats;
                var NewParticle2 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle2.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var BallTrailColor_2 = BallTrail.trails;
                BallTrailColor_2.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
            
            case "blue":
                other.gameObject.SetActive(false);
                Ballmats[1] = other.GetComponent<Renderer>().material;
                BallRenderer.materials = Ballmats;
                var NewParticle3 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle3.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                var BallTrailColor_3 = BallTrail.trails;
                BallTrailColor_3.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                break;
        }

        if (other.gameObject.name.Contains("Colorball"))
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 10);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<Text>().text = PlayerPrefs.GetInt("score").ToString();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("path"))
        {
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f,8f,0f);
            pathSpeed = pathSpeed * 2;

            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed = 10f;
            BallTrail.Stop();
            ballRotateSpeed = 1000f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("path"))
        {
            rb.isKinematic = _collider.isTrigger = true;
            pathSpeed = 30f;
            
            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed = 4f;

            Dust.transform.position = other.contacts[0].point + new Vector3(0f,0.3f,0f);
            Dust.GetComponent<Renderer>().material = BallRenderer.material;
            Dust.Play();
            BallTrail.Play();
            ballRotateSpeed = 500f;
        } 
    }
}
