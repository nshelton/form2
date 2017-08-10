using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector4Shader : MonoBehaviour {

	[SerializeField]
	private ComputeShader m_target;

	[SerializeField]
	private Vector4 m_theValue;

	[SerializeField]
	private string m_shaderVarName;

	[SerializeField]
	private float m_scale;

	public float X 
	{
		get{ return m_theValue.x;}
		set{ m_theValue.x = value;}
	}

	public float Y
	{
		get{ return m_theValue.y;}
		set{ m_theValue.y = value;}
	}

	public float Z 
	{
		get{ return m_theValue.z;}
		set{ m_theValue.z = value;}
	}

	public float W
	{
		get{ return m_theValue.w;}
		set{ m_theValue.w = value;}
	}

	// Update is called once per frame
	void Update () {
		
		m_target.SetVector(m_shaderVarName, m_scale * m_theValue);
	}
}
