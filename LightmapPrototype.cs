using System;
using UnityEngine;
/// <summary>
/// ������ͼԭ�ͣ��������������Ϣ
/// </summary>
public class LightmapPrototype
{
	public int rendererChildIndex = -1;

	public float scale = 1f;
    /// <summary>
    /// ������ͼ����
    /// </summary>
	public int lightmapIndex = -1;
    /// <summary>
    /// ������ͼƫ����
    /// </summary>
	public Vector4 lightmapTilingOffset;

	public int size = 16;
}
