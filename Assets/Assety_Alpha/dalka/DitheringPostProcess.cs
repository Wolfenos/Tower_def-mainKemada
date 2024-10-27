using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DitheringPostProcess : ScriptableRendererFeature
{
    class DitheringPass : ScriptableRenderPass
    {
        private Material ditheringMaterial;
        private RenderTargetIdentifier currentTarget;

        public DitheringPass(Material material)
        {
            ditheringMaterial = material;
        }

        public void Setup(RenderTargetIdentifier target)
        {
            currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Dithering Effect");
            cmd.Blit(currentTarget, currentTarget, ditheringMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] private Material ditheringMaterial;
    private DitheringPass ditheringPass;

    public override void Create()
    {
        ditheringPass = new DitheringPass(ditheringMaterial);
        ditheringPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        ditheringPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(ditheringPass);
    }
}
