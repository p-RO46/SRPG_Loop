using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    // 싱글톤 설정
    public static PoolManager Instance { get; private set; }

    // 프리팹을 키로 하여 각각의 풀을 저장하는 사전
    private Dictionary<GameObject, IObjectPool<GameObject>> _pools =
        new Dictionary<GameObject, IObjectPool<GameObject>>();

    private void Awake() => Instance = this;

    // 외부에서 "이 프리팹 좀 줘"라고 호출하는 함수
    public GameObject Get(GameObject prefab)
    {
        // 1. 해당 프리팹에 대한 풀이 아직 없다면 새로 생성
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = CreatePool(prefab);
        }

        // 2. 해당 풀에서 오브젝트 하나 꺼내기
        return _pools[prefab].Get();
    }

    // 오브젝트를 다시 풀로 돌려보낼 때 (반납)
    public void Release(GameObject prefab, GameObject instance)
    {
        if (_pools.ContainsKey(prefab))
        {
            _pools[prefab].Release(instance);
        }
    }

    // 실제 IObjectPool을 만드는 내부 로직
    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        IObjectPool<GameObject> pool = null; // 아래에서 할당하기 위해 미리 선언

        pool = new ObjectPool<GameObject>(
             createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.position = new Vector3(0f, -20f, 0f);
                // 생성 시점에 PooledObject 컴포넌트를 찾아서 풀 참조를 넣어줌
                var pooledScript = obj.GetComponent<PooledObject>();
                if (pooledScript == null) pooledScript = obj.AddComponent<PooledObject>();

                pooledScript.SetPool(pool); // "너의 주인은 이 풀이다"라고 지정
                return obj;
            },
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            maxSize: 30
        );

        return pool;
    }
}
