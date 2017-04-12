using System;
using System.IO;
using UnityEngine;
/// <summary>
/// ˮ������
/// </summary>
public class WaterData
{
    /// <summary>
    /// ��������ID
    /// </summary>
	public int ambienceSamplerID;
    /// <summary>
    /// ˮ��߶�
    /// </summary>
	public float height;
    /// <summary>
    /// ˮ�����ٶ�
    /// </summary>
	public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    /// <summary>
    /// ˮ����������
    /// </summary>
	public float waveScale = 0.02f;
    /// <summary>
    /// ˮ�崹ֱ��ɫ
    /// </summary>
	public Color horizonColor;
    /// <summary>
    /// ˮ�����ͼ��Դ·��
    /// </summary>
	public string depthMapPath = string.Empty;
    /// <summary>
    /// ����ɫ��Դ·��
    /// </summary>
	public string colorControlPath = string.Empty;
    /// <summary>
    /// ˮ�尼͹��ͼ·��
    /// </summary>
	public string waterBumpMapPath = string.Empty;
    /// <summary>
    /// ˮ��������
    /// </summary>
	public float waterVisibleDepth;
    /// <summary>
    /// ˮ��������ֵ
    /// </summary>
	public float waterDiffValue;
    /// <summary>
    /// ����Ť������
    /// </summary>
	public float reflDistort = 0.44f;
    /// <summary>
    /// ����
    /// </summary>
	public float refrDistort = 0.2f;
    /// <summary>
    /// ������ɫ��ͼ����
    /// </summary>
	public Texture2D colorControlMap;
    /// <summary>
    /// ˮ�尼͹��ͼ����
    /// </summary>
	public Texture2D bumpMap;
    /// <summary>
    /// ˮ�������ͼ����
    /// </summary>
	public Texture2D depthMap;
    /// <summary>
    /// ˮ��͸����
    /// </summary>
	public float alpha = 1f;
    /// <summary>
    /// ������ȡ������Ϣ
    /// </summary>
    /// <param name="br"></param>
	public void Read(BinaryReader br)
	{
		this.ambienceSamplerID = br.ReadInt32();
		this.height = br.ReadSingle();
		this.waveSpeed.x = br.ReadSingle();
		this.waveSpeed.y = br.ReadSingle();
		this.waveSpeed.z = br.ReadSingle();
		this.waveSpeed.w = br.ReadSingle();
		this.waveScale = br.ReadSingle();
		this.horizonColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
		this.waterVisibleDepth = br.ReadSingle();
		this.waterDiffValue = br.ReadSingle();
		long position = br.BaseStream.Position;
		if (br.ReadInt32() == 10015)
		{
			this.alpha = br.ReadSingle();
		}
		else
		{
			br.BaseStream.Position = position;
		}
		this.depthMapPath = br.ReadString();
		this.colorControlPath = br.ReadString();
		this.waterBumpMapPath = br.ReadString();
		this.colorControlMap = AssetLibrary.Load(this.colorControlPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		this.bumpMap = AssetLibrary.Load(this.waterBumpMapPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
		this.depthMap = AssetLibrary.Load(this.depthMapPath, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
	}
    /// <summary>
    /// �ͷŻ���
    /// </summary>
	public void Release()
	{
		this.colorControlMap = null;
		this.bumpMap = null;
		this.depthMap = null;
	}
}
