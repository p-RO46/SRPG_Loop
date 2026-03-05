using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePoolHandler : PooledObject
{
    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();

        // 루핑(Looping) 파티클이면 멈추지 않으므로 경고
        var main = _ps.main;
        if (main.loop)
        {
            // Debug.LogWarning($"{gameObject.name}이 루핑 중입니다. Stop Action이 작동하지 않을 수 있습니다.");
        }
    }

    // Stop Action이 Callback으로 설정되어 있으면 파티클 종료 시 자동 실행됨
    private void OnParticleSystemStopped()
    {
        // 부모 클래스(PooledObject)에 만든 반납 함수 호출
        ReturnToPool();
    }
}