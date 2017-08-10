using UnityEngine;

public class MultibufferParticles : MonoBehaviour
{
	private struct Particle
	{
		public Vector3 position;
		public Vector3 color;
	}

	private const int SIZE_PARTICLE = 24;

	[SerializeField]
	private int particleCount = 10000;

	[SerializeField]
	public Material material;

	[SerializeField]
	public ComputeShader computeShader;

	private int mComputeShaderKernelID;
	private const int WARP_SIZE = 128;
	private int mWarpCount;

	[SerializeField]
	private Vector3 camPos;

	[SerializeField]
	private float _FOV = 1.0f;


	[SerializeField]
	private float _StepRatio = 1f;
	public float StepRatio { get{return _StepRatio;} set{_StepRatio = value;} }

	[SerializeField]
	private float _Threshold = 1f;
	public float Threshold { get{return _Threshold;} set{_Threshold = value;} }

	[SerializeField]
	private float _jitter = 1f;
	
	[SerializeField]
	private float _fogAmount = 1f;


 	[ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] 
	[SerializeField]
	private Color _Color;

	[SerializeField]
	private int numBuffers = 8;

	[SerializeField]
	private int activeBuffer = 0;


	private Vector4 _RenderParam;

	public float AO1 
	{
		get{return _RenderParam.x;}
		set{_RenderParam.x = value;}
	}


	public float AO2 
	{
		get{return _RenderParam.y;}
		set{_RenderParam.y = value;}
	}


	public float AO3 
	{
		get{return _RenderParam.z;}
		set{_RenderParam.z = value;}
	}



	public float AO4 
	{
		get{return _RenderParam.w;}
		set{_RenderParam.w = value;}
	}



	private ComputeBuffer[] particleBuffers;

	void Start()
	{
		particleBuffers = new ComputeBuffer[numBuffers];
		mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);
		mComputeShaderKernelID = computeShader.FindKernel("CSMain");

		// make all our buffers
		for ( int i = 0; i < numBuffers; i ++)
		{
			Particle[] particleArray = new Particle[particleCount];
			particleBuffers[i] = new ComputeBuffer(particleCount, SIZE_PARTICLE);
			particleBuffers[i].SetData(particleArray);
		}
	}

	void OnDestroy()
	{
		for(int i = 0; i < particleBuffers.Length; i ++)
		{
			if ( particleBuffers[i] != null)
				particleBuffers[i].Release();
		}
	}
	
	void Update()
	{
		material.SetMatrix("modelToWorld", transform.localToWorldMatrix);
		material.SetColor("_Color", _Color);

		activeBuffer = (activeBuffer + 1) % numBuffers;

		// Send datas to the compute shader
		computeShader.SetFloat("Time", Time.time / 40f);
		computeShader.SetFloat("FOV", _FOV);
		computeShader.SetFloat("pointDim", Mathf.Sqrt(particleCount));
		computeShader.SetFloat("stepRatio", _StepRatio);

		computeShader.SetVector("camPos",
		 transform.transform.InverseTransformPoint(Camera.main.transform.position));

		Quaternion q =  Quaternion.Inverse(transform.rotation) *  Camera.main.transform.rotation;

		computeShader.SetVector("camQRot", new Vector4(q.x, q.y, q.z, q.w));
		computeShader.SetFloat("scale", transform.localToWorldMatrix.GetScale().x );

		computeShader.SetFloat("_Threshold", _Threshold);
		computeShader.SetFloat("_StepRatio", _StepRatio);
		computeShader.SetFloat("_jitter", _jitter);
		computeShader.SetVector("_RenderParam", _RenderParam);
		computeShader.SetFloat("_fogAmount", _fogAmount);

		// Update the Particles, only for the ative buffer
 
		computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffers[activeBuffer]);
		computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
	}

	void OnRenderObject()
	{
		// draw ALL the buffers
		for ( int i = 0; i < particleBuffers.Length; i++)
		{
			material.SetPass(0);
			material.SetBuffer("particleBuffer", particleBuffers[i]);
			Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
		}
	}
	
}
