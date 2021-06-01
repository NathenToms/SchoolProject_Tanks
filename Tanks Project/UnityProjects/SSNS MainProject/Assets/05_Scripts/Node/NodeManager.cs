//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using SNSSTypes;

//namespace Old_Node_Stuff
//{
//    /// <summary>
//    /// Handles switching from different nodes
//    /// </summary>
//    public class NodeManager : MonoBehaviour
//    {
//        public static NodeManager Instance;

//        [SerializeField] private GameObject node_Default;
//        [SerializeField] private GameObject node_Currrent;
//        [SerializeField] private GameObject node_Next;
//        [SerializeField] private GameObject node_Boss;


//        #region Constant Values

//        private const float MAX_SELECT_TIME = 10f;
//        private const float MIN_SELECT_TIME = 5f;
//        private const float SELECT_DELAY = 0.5f;

//        #endregion

//        #region Serialized Variables

//        [Space(5)]
//        [SerializeField] private NodeManagerUI nodeUI;

//        [Space(5)]
//        [SerializeField] private GameObject portalPrefab;

//        [SerializeField] private Node startNode;

//        [SerializeField] private int maxPortalDistance = 350;

//        #endregion

//        #region Node Storage

//        private Node currentNode;
//        private Node lastNode;

//        #endregion

//        private System.Random random;

//        #region Portal Stuff

//        private GameObject[] portals;

//        private bool rotateToPortal;

//        #endregion

//        #region Player Selection Variables

//        private int selectedIndex;

//        private bool selectingNode;
//        private bool nodeSelected;

//        #endregion

//        #region Time Variables

//        private float timeBeforeSelection;

//        private float timeSinceChange;

//        #endregion

//        /// <summary>
//        /// Begins next selection for next node
//        /// </summary>
//        public void BeginNodeSelection()
//        {
//            if (Choices.Length == 0) // If there is no choices left then game is over
//            {
//                GameManager.Instance.SwitchState(GameState.GAME_OVER);
//            }
//            else
//            {
//                selectingNode = true;
//                rotateToPortal = true;

//                if (Choices.Length > 1)
//                {
//                    timeBeforeSelection = MAX_SELECT_TIME;
//                }
//                else
//                {
//                    timeBeforeSelection = MIN_SELECT_TIME;
//                }

//                // Spawn the portals and update the information for the nodes
//                StartCoroutine(SpawnPortals());

//                // Selects the middle choice as default
//                NodeUpdate(Choices.Length / 2);

//                // Enable all node UI
//                //nodeUI.FadeGroups(true, true, true);
//            }
//        }

//        /// <summary>
//        /// Ends node selection and switches to node transition
//        /// </summary>
//        /// <param name="node">Next node to travel to</param>
//        public void TravelToNode(Node node)
//        {
//            // Resets all boolean variables
//            selectingNode = false;
//            nodeSelected = false;

//            // Stores the last node
//            //lastNode
//            lastNode = currentNode;

//            currentNode = node;

//            // Destroys the portal parent
//            if (portals != null)
//            {
//                Destroy(portals[0].transform.parent.gameObject);
//            }

//            lastNode.UpdateVisual(currentNode.NodeInfo, null);

//            currentNode.UpdateVisual(currentNode.NodeInfo, null);

//            // Hides all UI except the gunner
//            //nodeUI.FadeGroups(false, false, true);


//            // Switches state to node transition to load next area
//            GameManager.Instance.SwitchState(GameState.WARPING);
//        }

//        /// <summary>
//        /// Hides all UI that is tied to nodes
//        /// </summary>
//        public void HideAllNodeUI()
//        {
//            // Disable all node UI
//            //nodeUI.FadeGroups(false, false, false);

//            // Resets node color
//            if (lastNode != null)
//                lastNode.UpdateVisual(currentNode.NodeInfo, null);
//        }

//        #region Player Selection

//        /// <summary>
//        /// Select the node in the provided direction for the player role
//        /// </summary>
//        /// <param name="role">The player's role</param>
//        /// <param name="direction">The direction of the selection</param>
//        public void SelectNodeChoice(int direction)
//        {
//            if (selectingNode)
//            {
//                direction = Mathf.Clamp(direction, -1, 1);

//                if (timeSinceChange == 0)
//                {
//                    NodeUpdate(selectedIndex + direction);
//                }
//            }
//        }

//        /// <summary>
//        /// Updates the information for the node for the provided role
//        /// </summary>
//        /// <param name="role">The role to update</param>
//        /// <param name="newNodeIndex">The node to select</param>
//        private void NodeUpdate(int newNodeIndex)
//        {
//            newNodeIndex = Mathf.Clamp(newNodeIndex, 0, Choices.Length - 1);

//            timeSinceChange = SELECT_DELAY;
//            selectedIndex = newNodeIndex;
//            rotateToPortal = true;

//            // Updates the node UI for the pilot
//            #region UI

//            NodeInfo selectedNode = Choices[selectedIndex].NodeInfo;

//            NodeInfo leftNode = null;
//            NodeInfo rightNode = null;

//            int leftNodeP1 = selectedIndex - 1;
//            int rightNodeP1 = selectedIndex + 1;

//            if (leftNodeP1 >= 0)
//                leftNode = Choices[leftNodeP1].NodeInfo;

//            if (rightNodeP1 < Choices.Length)
//                rightNode = Choices[rightNodeP1].NodeInfo;

//            nodeUI.UpdatePilot(selectedNode, leftNode, rightNode);

//            currentNode.UpdateVisual(currentNode.NodeInfo, Choices[selectedIndex].NodeInfo);
//            Choices[selectedIndex].UpdateVisual(currentNode.NodeInfo, Choices[selectedIndex].NodeInfo);

//            #endregion
//        }

//        #endregion

//        #region Node Depth Finder

//        /// <summary>
//        /// Find all nodes at depth
//        /// </summary>
//        /// <param name="depth">The depth to find nodes at</param>
//        /// <returns></returns>
//        public Node[] FindNodes(int depth)
//        {
//            List<Node> nodes = new List<Node>();

//            FindNodes(ref nodes, depth, startNode);

//            return nodes.ToArray();
//        }

//        /// <summary>
//        /// Recursion method for finding nodes at depth
//        /// </summary>
//        /// <param name="nodes">Stores all nodes at depth</param>
//        /// <param name="depth">The depth to find it at</param>
//        /// <param name="node">Current node</param>
//        private void FindNodes(ref List<Node> nodes, int depth, Node node)
//        {
//            if (node == startNode)
//                nodes.Clear();

//            if (!nodes.Contains(node))
//            {
//                if (node == null) return;

//                if (node.Depth == depth)
//                {
//                    nodes.Add(node);
//                }
//                else
//                {
//                    foreach (Node n in node.Children)
//                    {
//                        FindNodes(ref nodes, depth, n);
//                    }
//                }
//            }
//        }

//        #endregion

//        /// <summary>
//        /// Spawns portals in-front of the player
//        /// </summary>
//        private IEnumerator SpawnPortals()
//        {
//            portals = new GameObject[Choices.Length];

//            // Create a parent for the portals
//            Transform parent = new GameObject("Portals").transform;

//            // Grabs the ship transform
//            Transform ship = GameManager.Instance.Player.transform;

//            while (Physics.Raycast(ship.transform.position, ship.forward, 1 << 8))
//            {
//                ship.transform.rotation *= Quaternion.Euler(10 * Time.deltaTime, 0, 0);

//                yield return new WaitForEndOfFrame();
//            }

//            // Vector the ship's forward direction
//            Vector3 forward = ship.forward * 300;

//            // Finds the far left side of the ship for placing the portals
//            Vector3 farLeft = -ship.right * maxPortalDistance / 2.0f;

//            // Loops through all choices and creates portals
//            for (int i = 0; i < Choices.Length; i++)
//            {
//                // Creates portal
//                GameObject portal = Instantiate(portalPrefab, parent);
//                portal.name = "[Portal] Choice: " + (i + 1);

//                Vector3 position = ship.right;

//                if (Choices.Length == 1)
//                    position *= DistanceBetweenPortals;
//                else
//                    position *= DistanceBetweenPortals * i;

//                // Sets the portal position
//                portal.transform.position = ship.position + forward + farLeft + position;

//                portals[i] = portal;
//            }
//        }

//        /// <summary>
//        /// Rotates the player ship to the portal
//        /// TODO: This probably should be handled in the player controller
//        /// </summary>
//        private void RotateToPortal()
//        {
//            if (portals == null || !rotateToPortal) return;

//            // Finds the direction to the portal
//            Vector3 portalDir = (portals[selectedIndex].transform.position - GameManager.Instance.Player.transform.position).normalized;
//            // Debug.Log(selectedIndex);

//            // Finds the angle between
//            float angle = Vector3.SignedAngle(GameManager.Instance.Player.transform.forward, portalDir, GameManager.Instance.Player.transform.up);

//            Vector2 dir = Vector2.zero;

//            // Find a direction based on the angle
//            if (Mathf.Abs(angle) > 0.5f)
//            {
//                float t = Mathf.Abs(angle) / DistanceBetweenPortals;

//                dir = new Vector2(Mathf.Lerp(0.3f, 3f, t), 0);

//                if (angle < 0)
//                {
//                    dir *= Vector2.left;
//                }

//                if (Mathf.Abs(dir.x) <= 0.3f)
//                {
//                    dir = Vector2.zero;
//                    rotateToPortal = false;
//                }
//            }
//            else
//            {
//                rotateToPortal = false;
//            }

//            // Steers the ship to the direction
//            // TODO
//            //  GameManager.Instance.shipController.SteerShip(dir);
//        }

//        private void Awake()
//        {
//            if (Instance != null)
//            {
//                Destroy(Instance.gameObject);
//            }

//            Instance = this;

//            timeBeforeSelection = MAX_SELECT_TIME;

//            currentNode = startNode;
//            lastNode = null;

//            //nodeUI.FadeGroups(false, false, true);
//            currentNode.UpdateVisual(currentNode.NodeInfo, null);
//        }

//        private void Start()
//        {
//            random = GameManager.Instance.Random;

//            // Generates all nodes
//            foreach (Node n in startNode)
//            {
//                n.GenerateNode();
//            }

//            AreaManager.Instance.AreaLoaded += HideAllNodeUI;
//        }

//        private void Update()
//        {
//            if (selectingNode)
//            {
//                timeBeforeSelection -= Time.deltaTime;

//                // If timer hits 0 select node based on player selections
//                if (timeBeforeSelection <= 0)
//                {
//                    timeBeforeSelection = 0;

//                    selectingNode = false;
//                    nodeSelected = true;

//                }

//                timeSinceChange -= Time.deltaTime;
//                if (timeSinceChange <= 0)
//                {
//                    timeSinceChange = 0;
//                }

//                // Update selection UI
//                nodeUI.UpdateTimer((int)timeBeforeSelection);
//            }

//            // Rotates to portal
//            RotateToPortal();

//            // Once the pilot has stopped moving and a node is selected ends node selection
//            if (nodeSelected && !rotateToPortal)
//            {
//                nodeSelected = false;
//                TravelToNode(Choices[selectedIndex]);
//            }
//        }

//        public Node[] Choices { get { return currentNode.Children; } }
//        public Node StartNode { get { return startNode; } }
//        public Node CurrentNode { get { return currentNode; } }

//        //public bool MapPreview { get { return mapPreview; } set { mapPreview = value; } }

//        private float DistanceBetweenPortals { get { return maxPortalDistance / (Choices.Length == 1 ? 2 : (Choices.Length - 1)); } }
//    }
//}
