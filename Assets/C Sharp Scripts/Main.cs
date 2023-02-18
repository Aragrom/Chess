using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Backbone for the game and how all components are connected.

    public Network network = null;
    public UserInterface userInterface = null;
    public Game game = null;
    public Evaluator evaluator = new Evaluator();
    public Mouse mouse = null;

    /*
    public Keyboard keyboard = null;

    public Controller controller = null;
    */

    public Player player = null;
    public AI ai = null;
    public NetworkedPlayer networkedPlayer = null;
    public NetworkedOpponent networkedOpponent = null;

    public float frameCount = 0;
    public float dt = 0.0f;
    public float fps = 0.0f;
    public float updateRate = 1.0f;  // 'underRate' update(s) per sec. (Update rate)
    public float instantFps = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        instantFps = 1.0f / Time.deltaTime;

        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }

        // Main execution/game loop
        // UserInterface.cs provides input.

        mouse.ControlledUpdate();
    }
}
