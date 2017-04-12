using System;
using UnityEngine;

public class TerrainConfig
{
    /// <summary>
    /// �������
    /// </summary>
	public int sceneWidth = 480;
    /// <summary>
    /// �����߶�
    /// </summary>
	public int sceneHeight = 480;
    /// <summary>
    /// �����߶�ͼ�ֱ���
    /// </summary>
	public float sceneHeightmapResolution = 480f;
    /// <summary>
    /// ����splat
    /// </summary>
	public Splat baseSplat = new Splat();
    /// <summary>
    /// ������۲��
    /// </summary>
	public Vector3 cameraLookAt = Vector3.zero;
    /// <summary>
    /// �������
    /// </summary>
	public float cameraDistance;
    /// <summary>
    /// �����x��ת
    /// </summary>
	public float cameraRotationX;
    /// <summary>
    /// �������Y��ת
    /// </summary>
	public float cameraRotationY;
    /// <summary>
    /// �������z��ת
    /// </summary>
	public float cameraRotationZ;
    /// <summary>
    /// ��Ч��ɫ
    /// </summary>
	public Color fogColor = new Color(0.266666681f, 0.8156863f, 1f);
    /// <summary>
    /// ��Ч�Ŀ�ʼ����
    /// </summary>
	public float startDistance = 6f;
    /// <summary>
    /// ��Ч��ȫ���ܶ�
    /// </summary>
	public float globalDensity = 1f;
    /// <summary>
    /// ��Ч�ĸ߶�������
    /// </summary>
	public float heightScale = 1f;
    /// <summary>
    /// ��Ч�߶�
    /// </summary>
	public float height = 110f;
    /// <summary>
    /// ��Чǿ��
    /// </summary>
	public Vector4 fogIntensity = new Vector4(1f, 1f, 1f, 1f);
    /// <summary>
    /// tile��Ƭ�ĳߴ�
    /// </summary>
	public int tileSize = 32;
    /// <summary>
    /// һ��Region����ÿ�ߵ�tile��
    /// </summary>
	public int tileCountPerSide;
    /// <summary>
    /// һ��Region������tile��
    /// </summary>
	public int tileCountPerRegion;
    /// <summary>
    /// region����ĳߴ�
    /// </summary>
	public int regionSize = 160;
    /// <summary>
    /// �߶�ͼ�ֱ���
    /// </summary>
	public int heightmapResolution = 32;
    /// <summary>
    /// ˮЧ�����ͼ�ֱ���
    /// </summary>
	public int waterDepthmapResolution = 64;
    /// <summary>
    /// ����ֱ���������
    /// </summary>
	public int gridResolution = 32;
    /// <summary>
    /// splatmap�ֱ���???
    /// </summary>
	public int splatmapResolution = 32;
    /// <summary>
    /// splatmap(layer)���� ���Ϊ4
    /// </summary>
	public int spaltsmapLayers = 4;

	public float blockHeight = 1f;
    /// <summary>
    /// �������߶�
    /// </summary>
	public float maxReachTerrainHeight = 200f;
    /// <summary>
    /// ����ߴ�
    /// </summary>
	public float gridSize = 1f;
    /// <summary>
    /// ����Ĭ�ϸ߶�ֵ
    /// </summary>
	public float defaultTerrainHeight = 50f;
    /// <summary>
    /// �������߶�ֵ
    /// </summary>
	public float maxTerrainHeight = 200f;
    /// <summary>
    /// ��Ƭ�޳�����
    /// </summary>
	public float tileCullingDistance = 100f;
    /// <summary>
    /// unit�޳�����
    /// </summary>
	public float unitCullingDistance = 30f;
    /// <summary>
    /// �����޳�����
    /// </summary>
	public float cullingBaseDistance = 10f;
    /// <summary>
    /// �޳��Ƕ�����
    /// </summary>
	public float cullingAngleFactor = 3f;
    /// <summary>
    /// �ӽ�LOD����????/
    /// </summary>
	public float viewAngleLodFactor = 2f;
    /// <summary>
    /// ��̬�޳�����
    /// </summary>
	public float dynamiCullingDistance = 15f;
    /// <summary>
    /// Ĭ���޳�����
    /// </summary>
	public float defautCullingFactor = 2f;
    /// <summary>
    /// ̫����/ƽ�й���շ���
    /// </summary>
	public Vector4 sunLightDir = new Vector4(-0.41f, 0.74f, 0.18f, 0f);
    /// <summary>
    /// ˮ�徵��(specular�����䷶Χ
    /// </summary>
	public float waterSpecRange = 46.3f;
    /// <summary>
    /// ˮ�徵��(specular������ǿ��
    /// </summary>
	public float waterSpecStrength = 0.84f;
    /// <summary>
    /// ˮ��(diffuse)�����䷶Χ
    /// </summary>
	public float waterDiffRange;
    /// <summary>
    /// ˮ��(diffuse)������ǿ��
    /// </summary>
	public float waterDiffStrength;
    /// <summary>
    /// ˮ���ٶ�
    /// </summary>
	public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    /// <summary>
    /// ˮ��������
    /// </summary>
	public float waveScale = 0.02f;
    /// <summary>
    /// ��ֱ��ɫ
    /// </summary>
	public Color horizonColor;
    /// <summary>
    /// ����������ͼ
    /// </summary>
	public Texture2D colorControl;
    /// <summary>
    /// ˮ�尼͹��ͼ
    /// </summary>
	public Texture2D waterBumpMap;
    /// <summary>
    /// Ĭ��ˮ��߶�
    /// </summary>
	public float defaultWaterHeight = 48f;
    /// <summary>
    /// ˮ��������
    /// </summary>
	public float waterVisibleDepth = 0.5f;
    /// <summary>
    /// ˮ��͸����(alpha)
    /// </summary>
	public float waterAlpha = 1f;
    /// <summary>
    /// ����Ť��
    /// </summary>
	public float reflDistort = 0.44f;
    /// <summary>
    /// ����Ť��
    /// </summary>
	public float refrDistort = 0.2f;
    /// <summary>
    /// ˮ��������ֵ?????
    /// </summary>
	public float waterDiffValue;
    /// <summary>
    /// ��ײ���㷶Χ
    /// </summary>
	public float collisionComputeRange = 3f;
    /// <summary>
    /// �Ƿ����õ��Դ
    /// </summary>
	public bool enablePointLight = true;
    /// <summary>
    /// ���Դ��С��Χ
    /// </summary>
	public float pointLightRangeMin = 2f;
    /// <summary>
    /// ���Դ���Χ
    /// </summary>
	public float pointLightRangeMax = 5.6f;
    /// <summary>
    /// ���Դǿ��
    /// </summary>
	public float pointLightIntensity = 1f;
    /// <summary>
    /// ���Դ��ɫ
    /// </summary>
	public Color pointLightColor = new Color(1f, 1f, 1f);
    /// <summary>
    /// ��ɫ���Դλ��
    /// </summary>
	public Vector3 rolePointLightPostion = new Vector3(-100.12f, -12.86f, 270.2f);
    /// <summary>
    /// ��ɫ���Դ��ɫ
    /// </summary>
	public Color rolePointLightColor = new Color(1f, 1f, 1f);
    /// <summary>
    /// ��ɫ���Դ��Χ
    /// </summary>
	public float rolePointLightRange = 19.7f;
    /// <summary>
    /// ��ɫ���Դǿ��
    /// </summary>
	public float rolePointLightIntensity = 2.68f;
    /// <summary>
    /// ��ɫ��ɫֵ
    /// </summary>
	public Color coolColor = new Color(1f, 1f, 1f, 1f);
    /// <summary>
    /// ůɫ��ɫֵ
    /// </summary>
	public Color warmColor = new Color(1f, 1f, 1f, 1f);
    /// <summary>
    /// ����(ֵ/���ͣ�������)
    /// </summary>
	public int weather;

    /// <summary>
    /// ���õ���
    /// </summary>
	public bool enableTerrain = true;
}
