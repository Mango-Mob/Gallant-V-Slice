using GEN.Users;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GEN.Nodes
{
    /**
     * A supplementary class which is exclusively used in StartNode.
     * @author : Michael Jordan
     * Used to store instantiated prefabs, at runtime, where each section corresponds to a new
     * prefab. This class ix exclusively used by StartNode.
     */
    public class Section
    {
        /** a public variable. 
         * How deap into the tree this section is 
         */
        public int depth { get; private set; }

        /** a public variable. 
         * The worldObject that is occupying this section.
         */
        public GameObject worldObject { get; private set; }

        /** a public variable. 
        * List of all remaining exits associated with the worldObject.
        */
        public List<ExitNode> exitList { get; private set; }

        /**
         * Initialises the section by setting all variables, instantiating the worldObject and destroying the enterance.
         * @param _object prefab used for reference.
         * @param _parent parent of the this section.
         * @param _depth depth of this section from the root section.
         */
        private void Initialise(GameObject _object, Transform _parent, int _depth)
        {
            //Set depth
            depth = _depth;
            
            //Update prefab section's depth
            _object.GetComponent<PrefabSection>().depth = depth;
            
            //Create the worldObject
            worldObject = GameObject.Instantiate(_object, _parent.transform);

            //Get the prefabSection of the world object
            PrefabSection prefabSection = worldObject.GetComponent<PrefabSection>();
            
            //Update position
            worldObject.transform.position += prefabSection.m_offset;

            //Set exit list
            exitList = new List<ExitNode>(prefabSection.GetComponentsInChildren<ExitNode>());

            //Disable all colliders for this object
            SetActiveColliders(false);

            //Destroy enterance
            UnityEngine.Object.DestroyImmediate(prefabSection.m_entry);
        }

        /**
         * Enables/Disables all colliders attached to the worldObject's exits.
         * @param _status status of all colliders.
         */
        public void SetActiveColliders(bool _status)
        {
            foreach (var item in exitList)
            {
                foreach (var collider in item.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = _status;
                }
            }
        }

        /** 
         * A constructor.
         * Generates the section with no exit or other section required.
         * @param _parent Transform of the parent object to attach to.
         * @param _prefab Gameobject to create a copy of in the game world.
         */
        public Section(Transform _parent, GameObject _prefab)
        {
            Initialise(_prefab, _parent, 0);
            worldObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        /** 
         * A constructor.
         * Generates the section at the selected exit of the parent provided.
         * @param _parent Section to generate from.
         * @param _selected Selected index for which exit to generate from.
         * @param _prefab Gameobject to create a copy of in the game world.
         */
        public Section(Section _parent, int _selected, GameObject _prefab)
        {
            //Find selected exit
            ExitNode exit = _parent.exitList[_selected];

            Initialise(_prefab, _parent.exitList[_selected].transform, _parent.depth + 1);

            worldObject.transform.localRotation = Quaternion.Euler(0, 180, 0);

            _parent.exitList.Remove(exit);
            UnityEngine.Object.DestroyImmediate(exit);
        }

        /** 
         * A constructor.
         * Generates the section at the provided exit, of the parent provided.
         * @param _parent Section to generate from.
         * @param _exit Actual exit to generate from.
         * @param _prefab Gameobject to create a copy of in the game world.
         */
        public Section(Section _parent, ExitNode _exit, GameObject _prefab)
        {
            Initialise(_prefab, _exit.transform, _parent.depth + 1);
            
            //Remove exit
            _parent.exitList.Remove(_exit);
            UnityEngine.Object.DestroyImmediate(_exit);
        }

    }

    /**
     * A component used to generate a full level of sections and a single cap.
     * A branching tree design for creating the level, while also detecting and avoiding any ingame collisions.
     */
    public class StartNode : MonoBehaviour
    {
        [Header("User Settings")]

        /** a public variable.
         * Seed to generate the level with.
         */
        [SerializeField][Tooltip("Seed to generate the level with.")] 
        public int m_seed = 0;

        /** a public variable. 
         * How many sections to generate from the start node before adding the cap.
         */
        [SerializeField][Tooltip("How many sections to generate from the start node before adding the cap.")]
        [Range(1, 20)] public int m_distance = 1;

        /** a public variable. 
         * Layers to check for, to disable all generation into a colliders of said layer.
         */
        [SerializeField][Tooltip("Layers to check for, to disable all generation into a colliders of said layer.")]
        public LayerMask m_layersToCheck = ~0;

        /** a public variable. 
         * Allow the node to generatte the whole level when the scene loads.
         */
        [Tooltip("Allow the node to generatte the whole level when the scene loads.")]
        public bool m_GenerateLevelOnAwake = false;

        /** a public variable. 
         * Allow the node to generatte a random seed when the scene loads.
         */
        [Tooltip("Allow the node to generatte a random seed when the scene loads.")]
        public bool m_GenerateSeedOnAwake = true;

        /** a private variable. 
         * An array of prefabs to use when generating the level.
         */
        [SerializeField][HideInInspector]
         private GameObject[] m_levelPrefabs;

        /** a private variable. 
         * A prefab to use when concluding a level.
         */
        [HideInInspector][SerializeField] 
        private GameObject m_levelEnd;

        /** a private variable. 
         * A list of dynamically created sections.
         */
        [HideInInspector][SerializeField] 
        private List<Section> m_levelSections = new List<Section>();

        /**
         * Awake function.
         * Called when the component is loaded into the scene (Immediately).
         */
        private void Awake()
        {
            if(m_GenerateSeedOnAwake)
                m_seed = (int)System.DateTime.Now.Ticks;

            if (m_GenerateLevelOnAwake)
            {
                Clear();
                Generate(m_seed);
            }
        }

        /**
         * Generate the level using the prefabs provided.
         * Starting from a randomly selected prefab, each designated exit is checked for another prefab.
         * Once a depth as been reached, of which is the distance from the start node, the node will attempt to place the cap section.
         * In the event of a cap not being able to placed, the function will keep going until another section has reached the depth 
         * requirement or until the section limit is met. The section limit is dynamically set to m_distance^2.
         * @param _seed seed to generate the level with (default = random).
         */
        public void Generate(int _seed = 0)
        {
            //Check for errors
            if(!IsArrayFull(m_levelPrefabs))
            {
                DestroyImmediate(this);
                Debug.LogError("<GEN> Level start was created without a complete list of prefabs.");
                return;
            }
    
            //Clear all previous sections.
            ClearSections();
    
            //Set seed and start timer
            DateTime start = DateTime.Now;
            m_seed = _seed;
            if (_seed == 0)
            {
                m_seed = (int)System.DateTime.Now.Ticks;
            }
            //Set seed
            UnityEngine.Random.InitState(m_seed);

            //Select a random initial prefab
            List<GameObject> prefabList = GetListOfValidPrefabs(transform, Quaternion.Euler(0, 180, 0));

            //Check for errors
            if (prefabList.Count == 0)
            {
                Debug.LogError("<GEN> Failed to generate level.");
                return;
            }

            //Select a random prefab
            int prefabSelect = UnityEngine.Random.Range(0, prefabList.Count);

            //Create the new section
            m_levelSections.Add(new Section(transform, prefabList[prefabSelect]));
            m_levelSections[0].worldObject.name = "<Gen> Section [0]";
    
            List<Section> currentSections = new List<Section>(m_levelSections);
            while (currentSections.Count > 0) //m_levelSections[m_levelSections.Count - 1].depth < m_distance && 
            {
                Physics.SyncTransforms();

                //Edge case
                if (m_distance - 1 == 0)
                {
                    GenerateCapRoom();
                    break;
                }

                //Safety Case
                if (m_levelSections.Count >= Mathf.Pow(m_distance, 2))
                {
                    Debug.LogError("<GEN> Failed to find the end of the level.");
                    break;
                }
                
                //Select a random section to add to
                int parentSelected = UnityEngine.Random.Range(0, currentSections.Count);

                //Select a random exit to add to
                int exitSelected = UnityEngine.Random.Range(0, currentSections[parentSelected].exitList.Count);
    
                //Get a list of valid prefabs
                prefabList = GetListOfValidPrefabs(currentSections[parentSelected].exitList[exitSelected].transform, Quaternion.Euler(0, 180, 0));          
    
                if(prefabList.Count > 0)
                {
                    //Select a random prefab
                    prefabSelect = UnityEngine.Random.Range(0, prefabList.Count);
                    //Create the new section
                    m_levelSections.Add(new Section(currentSections[parentSelected], exitSelected, prefabList[prefabSelect]));
                    m_levelSections[m_levelSections.Count - 1].worldObject.name = $"<Gen> Section [{m_levelSections.Count - 1}]";
                    m_levelSections[m_levelSections.Count - 1].worldObject.transform.SetParent(transform);

                    //Add to the list of branchable sections
                    currentSections.Add(m_levelSections[m_levelSections.Count - 1]);
                }
                else //remove from the list of potential exits
                    currentSections[parentSelected].exitList.RemoveAt(exitSelected);
                
                //Has a valid exit been found?
                if (m_levelSections[m_levelSections.Count - 1].depth >= m_distance - 1)
                {
                    if(GenerateCapRoom())
                    {
                        //Successfully created the cap
                        break;
                    }
                }
    
                //Remove all sections with no remaining exits
                for (int i = currentSections.Count - 1; i >= 0; i--)
                {
                    if (currentSections[i].exitList.Count == 0)
                    {
                        currentSections.RemoveAt(i);
                    }
                }
            }
            
            //Set up all sections to be useable.
            for (int i = 0; i < m_levelSections.Count; i++)
            {
                m_levelSections[i].SetActiveColliders(true);
            }

            //Log time taken
            Debug.Log($"Generate level in: {(DateTime.Now - start).TotalMilliseconds} ms");
        }

        /**
         * Generate the cap section.
         * Starts to check if the cap can be placed from each exit of the last section, then randomly selects a valid one.
         * @return status of the cap being generated.
         */
        private bool GenerateCapRoom()
        {
            Physics.SyncTransforms();
    
            PrefabSection owner = m_levelEnd.GetComponent<PrefabSection>();
            owner.Awake();
    
            //Search through the last added section for a valid exit
            List<ExitNode> exits = new List<ExitNode>(m_levelSections[m_levelSections.Count - 1].exitList);
            for (int i = exits.Count - 1; i >= 0; i--)
            {
                //Check if any colliders don't allow this exit
                foreach (var collider in owner.m_levelColliders)
                {
                    if (!collider.enabled)
                        continue; //to next collider
    
                    List<Collider> hitColliders = collider.IsOverlapping(exits[i].transform, Quaternion.Euler(0, 180, 0), m_layersToCheck);
                    if (hitColliders.Count > 0)
                    {
                        //Remove exit from list
                        exits.RemoveAt(i);
                        break;
                    }
                }
            }

            //if an exit exists
            if (exits.Count > 0)
            {
                //Select a random valid exit
                int select = UnityEngine.Random.Range(0, exits.Count);

                //Add the boss section to the world
                m_levelSections.Add(new Section(m_levelSections[m_levelSections.Count - 1], exits[select], m_levelEnd));
                m_levelSections[m_levelSections.Count - 1].worldObject.transform.SetParent(transform);
                return true;
            }

            return false;
        }

        /**
         * Copy this startNode's section/cap to the variables provided.
         * @param _sectionPrefabs list of sections prefabs to occupy  (output).
         * @param _endCapPrefab cap prefab to occupy (output).
         */
        public void Copy(out List<GameObject> _sectionPrefabs, out GameObject _endCapPrefab)
        {
            _sectionPrefabs = new List<GameObject>(m_levelPrefabs);
            _endCapPrefab = m_levelEnd;
        }

        /**
         * Paste the section/cap to this startNode.
         * @param _prefabs array of sections prefabs.
         * @param _endCap cap prefab.
         * @return status of the paste.
         */
        public bool Paste(GameObject[] _prefabs, GameObject _endCap)
        {
            if (!IsArrayFull(_prefabs))
            {
                Debug.LogError("<GEN> Level start was created without a complete list of prefabs.");
                return false;
            }

            m_levelPrefabs = _prefabs;
            m_levelEnd = _endCap;
            return true;
        }

        /**
         * Compares all prefabs to the world, to see if any are valid.
         * @param _parent parent transform to test each prefab from.
         * @param _prefabLocalRotation rotation of the prefab to use.
         * @return list of valid prefabs.
         */
        private List<GameObject> GetListOfValidPrefabs(Transform _parent, Quaternion _prefabLocalRotation)
        {
            //Select initial prefab
            List<GameObject> prefabList = new List<GameObject>(m_levelPrefabs);
            for (int i = prefabList.Count - 1; i >= 0; i--)
            {
                PrefabSection owner = prefabList[i].GetComponent<PrefabSection>();
                owner.Awake();
    
                //Check each collider for if the selected prefab is valid.
                foreach (var collider in owner.m_levelColliders)
                {
                    if (!collider.enabled)
                        continue;
    
                    List<Collider> hitColliders = collider.IsOverlapping(_parent, _prefabLocalRotation, m_layersToCheck);
                    if (hitColliders.Count > 0)
                    {
                        prefabList.RemoveAt(i);
                        break; //Stop looping
                    }
                }
            }
            return prefabList;
        }

        /**
         * Clear all children of this node.
         */
        public void Clear()
        {
            ClearSections();
            ErrorNode.CleanAll();
        }

        /**
         * Calculate the average position of all sections.
         * @return : (Vector3) average position of all sections.
         */
        public Vector3 CalculateAveragePosition()
        {
            Vector3 pos = Vector3.zero;
            foreach (var item in m_levelSections)
            {
                pos += item.worldObject.transform.position;
            }
            return (m_levelSections.Count > 0) ? pos / m_levelSections.Count : transform.position;
        }

        /**
         * Clear all sections saved inside this node.
         */
        private void ClearSections()
        {
            while(m_levelSections.Count > 0)
            {
                DestroyImmediate(m_levelSections[m_levelSections.Count - 1].worldObject);
                m_levelSections.RemoveAt(m_levelSections.Count - 1);
            }
    
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                child.SetParent(null);
                DestroyImmediate(child.gameObject);
            }
        }

        /**
         * Checks an array of prefabs to see if any are null.
         * @param _prefabs array of prefabs to check.
         * @return status of the array.
         */
        private bool IsArrayFull(GameObject[] _prefabs)
        {
            //Edge case
            if (_prefabs == null)
                return false;

            //Edge case
            if (_prefabs.Length == 0)
                return false;
    
            foreach (var item in _prefabs)
            {
                if (item == null)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * OnDrawGizmos function.
         * Draws when gizmos is enabled.
         */
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);

            Gizmos.DrawSphere(transform.position + transform.forward * 0.5f, 0.05f);
        }
    }

}