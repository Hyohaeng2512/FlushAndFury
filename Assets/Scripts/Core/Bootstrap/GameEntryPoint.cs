using FlushAndFury.Core.DI;
using UnityEngine;

namespace FlushAndFury.Core.Bootstrap
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private ProjectLifetimeScope projectLifetimeScope;

        private void Awake()
        {
            if (projectLifetimeScope == null)
            {
                projectLifetimeScope = FindAnyObjectByType<ProjectLifetimeScope>();
            }

            projectLifetimeScope?.Bootstrap();
        }
    }
}
