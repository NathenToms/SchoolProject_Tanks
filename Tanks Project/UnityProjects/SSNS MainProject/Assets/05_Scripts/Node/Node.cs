//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using SNSSTypes;

//namespace Old_Node_Stuff
//{
//    /// <summary>
//    /// Node Gameobject used for creating node tree using the ui
//    /// TODO: UI needs to be modified
//    /// </summary>
//    public class Node : MonoBehaviour, IEnumerable<Node>
//    {
//        private static int maxDepth;

//        [SerializeField] GameObject linePrefab;

//        [Header("Visual Image Components")]
//        [SerializeField] private Text nodeName;

//        [Tooltip("Order is: Default, Current, Next, Boss")]
//        [SerializeField] private GameObject[] prefabs;

//        [SerializeField] private GameObject visual;

//        [Space(5)]
//        [SerializeField] private NodeInfo nodeInfo;

//        public void UpdateVisual(NodeInfo current, NodeInfo next)
//        {
//            // TODO: Change this later for when effects are added
//            if (nodeInfo.type == AreaType.Boss || nodeInfo.type == AreaType.MiniBoss) return;

//            Destroy(visual.gameObject);

//            if (current != null && current.name == nodeInfo.name)
//            {
//                visual = Instantiate(prefabs[1], transform);
//            }
//            else if (next != null && next.name == nodeInfo.name)
//            {
//                visual = Instantiate(prefabs[2], transform);
//            }
//            else
//            {
//                visual = Instantiate(prefabs[0], transform);
//            }

//            visual.transform.SetAsLastSibling();
//            if (visual.transform.Find("NodeSprite").transform.Find("Name").TryGetComponent(out Text text))
//            {
//                text.text = NodeName;
//            }
//        }

//        /// <summary>
//        /// Generate randomized node
//        /// </summary>
//        public void GenerateNode()
//        {
//            System.Random random = GameManager.Instance.Random;

//            // Creates random name
//            #region Name

//            char letter = (char)('a' + random.Next(26));
//            letter = char.ToUpper(letter);

//            nodeInfo.name = letter + "-" + random.Next(20);

//            name = "[Node] " + nodeInfo.name;

//            nodeName.text = NodeName;

//            #endregion

//            if (nodeInfo.reward == null)
//                nodeInfo.reward = RewardManager.Instance.GetReward(false);

//            // TODO: Make it so that switching nodes is never the same skybox
//            if (nodeInfo.skybox != -1)
//                nodeInfo.skybox = random.Next(SkyboxManager.Instance.SkyboxAmount);
//        }

//        private void Awake()
//        {
//            if (Depth > maxDepth)
//                maxDepth = Depth;
//        }

//        private void Start()
//        {
//            // Creates lines going to each children
//            foreach (Node node in Children)
//            {
//                RectTransform rect = Instantiate<GameObject>(linePrefab, transform).GetComponent<RectTransform>();

//                if (rect.TryGetComponent(out Image image))
//                {
//                    Color color = image.color;
//                    color.a = 0.05f;
//                    image.color = color;
//                }

//                float distance = Vector2.Distance(transform.localPosition, node.transform.localPosition);

//                Vector2 dir = node.transform.position - transform.position;
//                float angle = Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI;

//                rect.sizeDelta = new Vector2(distance, 10);
//                rect.Rotate(new Vector3(0, 0, angle));
//            }

//            Destroy(visual.gameObject);

//            // Changes node color depending on type
//            switch (Type)
//            {
//                case AreaType.MiniBoss:
//                case AreaType.Boss:
//                    visual = Instantiate(prefabs[3], transform);
//                    break;
//                default:
//                    visual = Instantiate(prefabs[0], transform);
//                    break;
//            }
//        }

//        /// <summary>
//        /// Used for looping through all nodes
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerator<Node> GetEnumerator()
//        {
//            return new NodeEnumerator(NodeManager.Instance.StartNode);
//        }

//        /// <summary>
//        /// Used for looping through all nodes
//        /// </summary>
//        /// <returns></returns>
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        #region Variable Getters & Setters

//        public int MaxDepth { get { return maxDepth; } }
//        public NodeInfo NodeInfo { get { return nodeInfo; } }
//        public string NodeName { get { return nodeInfo.name; } }
//        public int Depth { get { return nodeInfo.depth; } }
//        public int Skybox { get { return nodeInfo.skybox; } }
//        public AreaType Type { get { return nodeInfo.type; } }
//        public NodeEvent Event { get { return nodeInfo.nodeEvent; } }
//        public Reward Reward { get { return nodeInfo.reward; } }
//        public Node[] Children { get { return nodeInfo.children; } }

//        #endregion

//        /// <summary>
//        /// Allows looping through all connected nodes
//        /// </summary>
//        private class NodeEnumerator : IEnumerator<Node>
//        {
//            private Node startNode;
//            private Node currentNode;
//            private int currentDepth;
//            private Node[] depthNodes;
//            private int depthIndex;

//            public Node Current => currentNode;

//            object IEnumerator.Current => currentNode;

//            public NodeEnumerator(Node sNode)
//            {
//                startNode = sNode;

//                Reset();
//            }

//            public void Dispose()
//            {
//                startNode = null;
//                currentNode = null;
//                currentDepth = 0;
//                depthNodes = null;
//            }

//            public bool MoveNext()
//            {
//                if (currentDepth == startNode.Depth)
//                {
//                    currentNode = startNode;

//                    currentDepth++;

//                    depthNodes = NodeManager.Instance.FindNodes(currentDepth);
//                }
//                else
//                {

//                    if (depthNodes.Length == 0) return false;

//                    currentNode = depthNodes[depthIndex];

//                    depthIndex++;

//                    if (depthIndex >= depthNodes.Length)
//                    {
//                        currentDepth++;

//                        depthNodes = NodeManager.Instance.FindNodes(currentDepth);

//                        depthIndex = 0;
//                    }
//                }

//                if (currentNode == null) return false;

//                return true;
//            }

//            public void Reset()
//            {
//                currentNode = null;
//                currentDepth = startNode.Depth;
//                depthNodes = null;
//                depthIndex = 0;
//            }
//        }
//    }

//    /// <summary>
//    /// Holds all the information for each node
//    /// </summary>
//    [System.Serializable]
//    public class NodeInfo
//    {
//        [HideInInspector] public string name;
//        public int depth;
//        public int skybox = -1;
//        public AreaType type;
//        public NodeEvent nodeEvent;
//        public Reward reward;
//        public Node[] children;
//    }
//}