using System.Collections;
using UnityEngine;

public class MeshParticleView : BaseParticleView
{
    [SerializeField] private PSMeshRendererUpdater _meshParticleUpdater;
    [SerializeField] private GameObject _neitralMesh;

    public void SetMesh(GameObject objectToParticle)
    {
        _meshParticleUpdater.UpdateMeshEffect(objectToParticle);
    }

    public void SetNeitralMesh()
    {
        _meshParticleUpdater.UpdateMeshEffect(_neitralMesh.gameObject);
    }
}