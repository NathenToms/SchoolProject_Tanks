//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using SNSSTypes;
//using System.Reflection;

//namespace Old_Node_Stuff
//{

//    /// <summary>
//    /// Struct that manages all node UI elements
//    /// </summary>
//    [System.Serializable]
//    public struct NodeManagerUI
//    {
//        // Structs that contain UI elements
//        [SerializeField] private SelectionInfoUI selectionUI;
//        [SerializeField] private PilotNodeUI pilotNodeUI;
//        [SerializeField] private GunnerNodeUI gunnerNodeUI;

//        /// <summary>
//        /// Update the pilot's node information
//        /// </summary>
//        /// <param name="selectedNode">The pilot's currently selected node</param>
//        /// <param name="leftNode">The node that is on the left side</param>
//        /// <param name="rightNode">The node that is on the right side</param>
//        public void UpdatePilot(NodeInfo selectedNode, NodeInfo leftNode, NodeInfo rightNode)
//        {
//            #region Left + Right Node Info

//            // Hides the left or right node info if there is none
//            pilotNodeUI.leftNodeName.gameObject.SetActive(leftNode != null);
//            pilotNodeUI.rightNodeName.gameObject.SetActive(rightNode != null);

//            // Updates the left and right node info
//            if (leftNode != null)
//                pilotNodeUI.leftNodeName.text = leftNode.name;

//            if (rightNode != null)
//                pilotNodeUI.rightNodeName.text = rightNode.name;

//            #endregion

//            // Updates the selected node info
//            #region Node Selection Info

//            pilotNodeUI.nodeName.text = selectedNode.name;
//            pilotNodeUI.nodeType.text = "Type: " + System.Enum.GetName(typeof(AreaType), selectedNode.type);

//            if (selectedNode.reward)
//                pilotNodeUI.nodeReward.text = selectedNode.reward.rewardName;

//            #endregion
//        }

//        /// <summary>
//        /// Updates the selection timer
//        /// </summary>
//        /// <param name="time">The time that the selection is at</param>
//        public void UpdateTimer(int time)
//        {
//            selectionUI.confirmTime.text = time.ToString("00"); // Formats it to double digits
//        }

//        /// <summary>
//        /// Shows certain UI elements
//        /// </summary>
//        /// <param name="selection">Whether to show the selection UI</param>
//        /// <param name="pilot">Whether to show the pilot UI</param>
//        /// <param name="gunner">Whether to show the gunner UI</param>
//        public void FadeGroups(bool selection, bool pilot, bool gunner)
//        {
//            selectionUI.groupFade.FadeActive(selection);
//            pilotNodeUI.groupFade.FadeActive(pilot);
//            gunnerNodeUI.groupFade.FadeActive(gunner);

//            gunnerNodeUI.redicle.enabled = !gunner;
//        }
//    }

//    /// <summary>
//    /// Struct for containing the selection info UI
//    /// </summary>
//    [System.Serializable]
//    public struct SelectionInfoUI
//    {
//        public Fade groupFade;

//        public Text confirmTime;
//    }

//    /// <summary>
//    /// Struct for containing the pilot node UI
//    /// </summary>
//    [System.Serializable]
//    public struct PilotNodeUI
//    {
//        public Fade groupFade;

//        [Header("Selected Node UI")]
//        public Text nodeName;
//        public Text nodeType;
//        public Text nodeDifficulty;
//        public Text nodeReward;

//        [Header("Other Nodes")]
//        public Text leftNodeName;
//        public Text rightNodeName;
//    }

//    /// <summary>
//    /// Struct for containing the gunner node UI
//    /// </summary>
//    [System.Serializable]
//    public struct GunnerNodeUI
//    {
//        public Fade groupFade;

//        public Image redicle;
//    }
//}
