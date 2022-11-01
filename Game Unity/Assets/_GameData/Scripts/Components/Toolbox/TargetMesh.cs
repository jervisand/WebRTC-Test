using UnityEngine;
using System.Collections;

namespace Nagih
{
    public class TargetMesh : MonoBehaviour
    {
        public MeshType Type;
        [DrawIf("Type", MeshType.MeshRenderer)] public MeshRenderer MeshRenderer;
        [DrawIf("Type", MeshType.MeshRenderer)] public MeshFilter MeshFilter;
        [DrawIf("Type", MeshType.SkinnedMeshRenderer)] public SkinnedMeshRenderer SkinnedMeshRenderer;

        public enum MeshType
        {
            MeshRenderer,
            SkinnedMeshRenderer
        }
    }
}