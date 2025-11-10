using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler instance;
    
    public static ObjectPooler Instance
    {
        get
        {
            if (instance == null)
            {
                // Tự động tạo nếu chưa tồn tại
                GameObject go = new GameObject("ObjectPooler_Auto");
                instance = go.AddComponent<ObjectPooler>();
            }
            return instance;
        }
    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePools();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            RegisterPool(pool.tag, pool.prefab, pool.size);
        }
    }

    /// <summary>
    /// Đăng ký pool runtime
    /// </summary>
    public void RegisterPool(string tag, GameObject prefab, int size)
    {
        if (poolDictionary == null)
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
        
        if (prefabDictionary == null)
            prefabDictionary = new Dictionary<string, GameObject>();

        if (poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool '{tag}' already registered!");
            return;
        }

        prefabDictionary[tag] = prefab;
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            objectPool.Enqueue(obj);
        }

        poolDictionary[tag] = objectPool;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, GameObject prefab = null)
    {
        // 1. Kiểm tra pool có tồn tại không
        if (!poolDictionary.ContainsKey(tag))
        {
            if (prefab != null)
            {
                RegisterPool(tag, prefab, 10); // Tự đăng ký nếu có prefab
            }
            else
            {
                Debug.LogError($"Pool '{tag}' not found and no prefab provided!");
                return null;
            }
        }

        GameObject objectToSpawn;

        // 2. KIỂM TRA MỚI: Nếu pool rỗng
        if (poolDictionary[tag].Count == 0)
        {
            // === LOGIC MỞ RỘNG (DYNAMIC EXPANSION) ===
            Debug.LogWarning($"Pool '{tag}' is empty! Expanding pool by 1.");
            
            // Lấy prefab đã đăng ký
            if (!prefabDictionary.ContainsKey(tag))
            {
                Debug.LogError($"Pool '{tag}' has no prefab registered! Cannot expand.");
                return null;
            }
            GameObject prefabToSpawn = prefabDictionary[tag];

            // Tạo một object mới
            objectToSpawn = Instantiate(prefabToSpawn);
            
            // (Lưu ý: Object này không nằm trong pool,
            // nhưng khi nó được ReturnToPool, nó sẽ TỰ ĐỘNG
            // được thêm vào queue, mở rộng pool size)
        }
        else
        {
            // === LOGIC CŨ: Lấy từ pool ===
            objectToSpawn = poolDictionary[tag].Dequeue();
        }
        
        // 3. SET ACTIVE VÀ SETUP (Dùng chung cho cả 2 trường hợp)
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        // (Tùy chọn: Set parent về null để nó không bị
        // dính theo ObjectPooler khi di chuyển)
        // objectToSpawn.transform.SetParent(null); 

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj?.OnObjectSpawn();
        
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        Debug.Log("Returning to the pool");
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool '{tag}' doesn't exist!");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[tag].Enqueue(obj);
        
    }
    public void ResetAllPools()
    {
        foreach (var pair in poolDictionary)
        {
            Queue<GameObject> queue = pair.Value;
        
            // Cần tạo một list tạm, vì không thể sửa Queue khi đang duyệt
            List<GameObject> allObjects = new List<GameObject>(queue);
        
            // Reset mọi object trong pool này
            foreach (GameObject obj in allObjects)
            {
                if (obj.activeSelf)
                {
                    // Nếu nó đang active, trả nó về pool
                    ReturnToPool(pair.Key, obj); 
                }
            }
        }
    }
}