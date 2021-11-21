using GEN.Users;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GEN.Nodes
{
    /****************
     * Section : A supplementary class that is exclusively used in StartNode.
     * @author : Michael Jordan
     * @file : StartNode.cs
     * @year : 2021
     */
    public class Section
    {
        //How deap into the tree this section is
        public int depth { get; private set; }
        //The worldObject that is occupying this section
        public GameObject worldObject { get; private set; }

        //List of all remaining exits associated with the worldObject
        public List<ExitNode> exitList { get; private set; }

        /*******************
         * Initialise : Initialises the section by setting all variables, instantiating the worldObject and destroying the enterance.
         * @author : Michael Jordan
         * @param : (GameObject) prefab used for reference.
         * @param : (Transform) parent of the this section.
         * @param : (int) depth of this section from the root section.
         */
        private void Initialise(GameObject _object, Transform parent, int _depth)
        {
            //Set depth
            depth = _depth;
            
            //Update prefab section's depth
            _object.GetComponent<PrefabSection>().depth = depth;
            
            //Create the worldObject
            worldObject = GameObject.Instantiate(_object, parent.transform);

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

        /*******************
         * SetActiveColliders : Enables/Disables all colliders attached to the worldObject's exits.
         * @author : Michael Jordan
         * @param : (bool) active status of all colliders.
         */
        public void SetActiveColliders(bool status)
        {
            foreach (var item in exitList)
            {
                foreach (var collider in item.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = status;
                }
            }
        }

        //Constructor
        public Section(Transform parent, GameObject _prefab)
        {
            Initialise(_prefab, parent, 0);
            worldObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        //Constructor
        public Section(Section _parent, int _selectedExit, GameObject _prefab)
        {
            //Find selected exit
            ExitNode exit = _parent.exitList[_selectedExit];

            Initialise(_prefab, _parent.exitList[_selectedExit].transform, _parent.depth + 1);

            worldObject.transform.localRotation = Quaternion.Euler(0, 180, 0);

            _parent.exitList.Remove(exit);
            UnityEngine.Object.DestroyImmediate(exit);
        }

        //Constructor
        public Section(Section _parent, ExitNode _exit, GameObject _prefab)
        {
            Initialise(_prefab, _exit.transform, _parent.depth + 1);
            
            //Remove exit
            _parent.exitList.Remove(_exit);
            UnityEngine.Object.DestroyImmediate(_exit);
        }

    }

    /****************
     * StartNode : A component used to generate a full level of sections and a single cap.
     * @author : Michael Jordan
     * @file : StartNode.cs
     * @year : 2021
     */
    public class StartNode : MonoBehaviour
    {
        [Header("User Settings")]
        [Tooltip("Seed to generate the level with.")]
        [SerializeField] public int m_seed = 0;

        [Range(1, 20)]
        [Tooltip("How many sections to generate from the start node before adding the cap.")]
        [SerializeField] public int m_distance = 1;

        [Tooltip("Layers to check for, to disable all generation into a colliders of said layer.")]
        [SerializeField] public LayerMask m_layersToCheck = ~0;
    
        [Tooltip("Allow the node to generatte the whole level when the scene loads.")]
        public bool m_GenerateLevelOnAwake = false;
        [Tooltip("Allow the node to generatte a random seed when the scene loads.")]
        public bool m_GenerateSeedOnAwake = true;

        [HideInInspector]
        [SerializeField] private GameObject[] m_levelPrefabs;
        [HideInInspector]
        [SerializeField] private GameObject m_levelEnd;
        [HideInInspector]
        [SerializeField] private List<Section> m_levelSections = new List<Section>();

        //Called when the component is loaded into the scene (Immediately).
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

        /*******************
         * Generate : Generate the level using the prefabs provided.
         * @author : Michael Jordan
         * @param : (int) seed to generate the level with (default = random).
         */
        public void Generate(int seed = 0)
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
            m_seed = seed;
            if (seed == 0)
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

        /*******************
         * GenerateBossRoom : Generate the cap section.
         * @author : Michael Jordan
         * @return : (bool) status of the cap being generated.
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

        /*******************
         * Copy : Copy this startNode's section/cap to the variables provided.
         * @author : Michael Jordan
         * @param : (out List<GameObject>) list of sections prefabs to occupy.
         * @param : (out GameObject) cap prefab to occupy.
         */
        public void Copy(out List<GameObject> _sectionPrefabs, out GameObject _endCapPrefab)
        {
            _sectionPrefabs = new List<GameObject>(m_levelPrefabs);
            _endCapPrefab = m_levelEnd;
        }

        /*******************
         * Paste : Paste the section/cap to this startNode.
         * @author : Michael Jordan
         * @param : (GameObject[]) array of sections prefabs.
         * @param : (GameObject) cap prefab.
         * @return : (bool) status of the paste.
         */
        public bool Paste(GameObject[] prefabs, GameObject endCap)
        {
            if (!IsArrayFull(prefabs))
            {
                Debug.LogError("<GEN> Level start was created without a complete list of prefabs.");
                return false;
            }

            m_levelPrefabs = prefabs;
            m_levelEnd = endCap;
            return true;
        }

        /*******************
         * GetListOfValidPrefabs : Compares all prefabs to the world, to see if any are valid.
         * @author : Michael Jordan
         * @param : (Transform) parent transform to test each prefab from.
         * @param : (Quaternion) rotation of the prefab to use.
         * @return : (List<GameObject>) list of valid prefabs.
         */
        private List<GameObject> GetListOfValidPrefabs(Transform parent, Quaternion prefabLocalRotation)
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
    
                    List<Collider> hitColliders = collider.IsOverlapping(parent, prefabLocalRotation, m_layersToCheck);
                    if (hitColliders.Count > 0)
                    {
                        prefabList.RemoveAt(i);
                        break; //Stop looping
                    }
                }
            }
            return prefabList;
        }

        /*******************
         * Clear : Clear all children of this node.
         * @author : Michael Jordan
         */
        public void Clear()
        {
            ClearSections();
            ErrorNode.CleanAll();
        }

        /*******************
         * CalculateAveragePosition : Calculate the average position of all sections.
         * @author : Michael Jordan
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

        /*******************
         * ClearSections : Clear all sections saved inside this node.
         * @author : Michael Jordan
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

        /*******************
         * IsArrayFull : Checks an array of prefabs to see if any are null.
         * @author : Michael Jordan
         * @param : (GameObject[]) array to check.
         * @return : (bool) status of the array.
         */
        private bool IsArrayFull(GameObject[] prefabs)
        {
            //Edge case
            if (prefabs == null)
                return false;

            //Edge case
            if (prefabs.Length == 0)
                return false;
    
            foreach (var item in prefabs)
            {
                if (item == null)
                {
                    return false;
                }
            }
            return true;
        }

        //Draws when gizmos is enabled
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Handles.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
            Handles.ConeHandleCap(0, transform.position + transform.forward * 0.5f, Quaternion.LookRotation(transform.forward, Vector3.up), 0.20f, EventType.Repaint);
        }
    }

}