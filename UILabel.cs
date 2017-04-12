using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Label"), ExecuteInEditMode]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline
	}

	public enum Overflow
	{
		ShrinkContent,
		ClampContent,
		ResizeFreely,
		ResizeHeight
	}

	public enum Crispness
	{
		Never,
		OnDesktop,
		Always
	}

	public delegate void CallLoadAsset(string strFileName, AssetCallBack callback);

	public delegate string OnQueryString(string strText);

	public UILabel.Crispness keepCrispWhenShrunk = UILabel.Crispness.OnDesktop;

	[HideInInspector, SerializeField]
	public Font mTrueTypeFont;

	[HideInInspector, SerializeField]
	private UIFont mFont;

	[HideInInspector, Multiline(6), SerializeField]
	private string mText = string.Empty;

	[HideInInspector, SerializeField]
	private int mFontSize = 16;

	[HideInInspector, SerializeField]
	private FontStyle mFontStyle;

	[HideInInspector, SerializeField]
	private NGUIText.Alignment mAlignment;

	[HideInInspector, SerializeField]
	private bool mEncoding = true;

	[HideInInspector, SerializeField]
	private int mMaxLineCount;

	[HideInInspector, SerializeField]
	private UILabel.Effect mEffectStyle;

	[HideInInspector, SerializeField]
	private Color mEffectColor = Color.black;

	[HideInInspector, SerializeField]
	private NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;

	[HideInInspector, SerializeField]
	private Vector2 mEffectDistance = Vector2.one;

	[HideInInspector, SerializeField]
	private UILabel.Overflow mOverflow;

	[HideInInspector, SerializeField]
	private Material mMaterial;

	[HideInInspector, SerializeField]
	private bool mApplyGradient;

	[HideInInspector, SerializeField]
	private Color mGradientTop = Color.white;

	[HideInInspector, SerializeField]
	private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	[HideInInspector, SerializeField]
	private int mSpacingX;

	[HideInInspector, SerializeField]
	private int mSpacingY;

	[HideInInspector, SerializeField]
	private bool mShrinkToFit;

	[HideInInspector, SerializeField]
	private int mMaxLineWidth;

	[HideInInspector, SerializeField]
	private int mMaxLineHeight;

	[HideInInspector, SerializeField]
	private float mLineWidth;

	[HideInInspector, SerializeField]
	private bool mMultiline = true;

	[HideInInspector, SerializeField]
	private bool isChanged;

	[HideInInspector, SerializeField]
	public string mOriginalText = string.Empty;

	[HideInInspector, SerializeField]
	public bool mIntConvert;

	public bool needChangeWord = true;

	private Font mActiveTTF;

	private float mDensity = 1f;

	private bool mShouldBeProcessed = true;

	private string mProcessedText = string.Empty;

	private bool mPremultiply;

	private Vector2 mCalculatedSize = Vector2.zero;

	private float mScale = 1f;

	private int mPrintedSize;

	private int mLastWidth;

	private int mLastHeight;

	private static string[] separater = new string[]
	{
		"\\n"
	};

	private static StringBuilder sb = new StringBuilder();

	private static BetterList<UILabel> mList = new BetterList<UILabel>();

	private static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	public static UILabel.CallLoadAsset monLoadAsset = null;

	private static StringBuilder mstrbuilder = new StringBuilder();

	public static UILabel.OnQueryString moQuery = null;

	public static UILabel.OnQueryString intConvertFunc = null;

	private static BetterList<Vector3> mTempVerts = new BetterList<Vector3>();

	private static BetterList<int> mTempIndices = new BetterList<int>();

	private Action m_symbolOffset;

	public string originalText
	{
		get
		{
			return this.mOriginalText;
		}
		set
		{
			if (this.mOriginalText == value)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(this.mOriginalText))
				{
					this.mOriginalText = string.Empty;
					this.shouldBeProcessed = true;
					this.ProcessAndRequest();
				}
			}
			else if (this.mOriginalText != value)
			{
				this.mOriginalText = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	public bool wordIsChanged
	{
		get
		{
			return this.isChanged;
		}
		set
		{
			this.isChanged = value;
		}
	}

	private bool shouldBeProcessed
	{
		get
		{
			return this.mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				this.mChanged = true;
				this.mShouldBeProcessed = true;
			}
			else
			{
				this.mShouldBeProcessed = false;
			}
		}
	}

	public override bool isAnchoredHorizontally
	{
		get
		{
			return base.isAnchoredHorizontally || this.mOverflow == UILabel.Overflow.ResizeFreely;
		}
	}

	public override bool isAnchoredVertically
	{
		get
		{
			return base.isAnchoredVertically || this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight;
		}
	}

	public override Material material
	{
		get
		{
			if (this.mMaterial != null)
			{
				return this.mMaterial;
			}
			if (this.mFont != null)
			{
				return this.mFont.material;
			}
			if (this.mTrueTypeFont != null)
			{
				return this.mTrueTypeFont.material;
			}
			return null;
		}
		set
		{
			if (this.mMaterial != value)
			{
				this.MarkAsChanged();
				this.mMaterial = value;
				this.MarkAsChanged();
			}
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font
	{
		get
		{
			return this.bitmapFont;
		}
		set
		{
			this.bitmapFont = value;
		}
	}

	public UIFont bitmapFont
	{
		get
		{
			return this.mFont;
		}
		set
		{
			if (this.mFont != value)
			{
				base.RemoveFromPanel();
				this.mFont = value;
				this.mTrueTypeFont = null;
				this.MarkAsChanged();
			}
		}
	}

	public Font trueTypeFont
	{
		get
		{
			if (this.mTrueTypeFont != null)
			{
				return this.mTrueTypeFont;
			}
			return (!(this.mFont != null)) ? null : this.mFont.dynamicFont;
		}
		set
		{
			if (this.mTrueTypeFont != value)
			{
				this.SetActiveFont(null);
				base.RemoveFromPanel();
				this.mTrueTypeFont = value;
				this.shouldBeProcessed = true;
				this.mFont = null;
				this.SetActiveFont(value);
				this.ProcessAndRequest();
				if (this.mActiveTTF != null)
				{
					base.MarkAsChanged();
				}
			}
		}
	}

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (!(this.mFont != null)) ? this.mTrueTypeFont : this.mFont;
		}
		set
		{
			UIFont uIFont = value as UIFont;
			if (uIFont != null)
			{
				this.bitmapFont = uIFont;
			}
			else
			{
				this.trueTypeFont = (value as Font);
			}
		}
	}

	public string text
	{
		get
		{
			return this.mText;
		}
		set
		{
			if (this.mText == value)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(this.mText))
				{
					this.mText = string.Empty;
					this.shouldBeProcessed = true;
					this.ProcessAndRequest();
				}
			}
			else if (this.mText != value)
			{
				this.originalText = string.Empty;
				this.mText = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
			if (this.autoResizeBoxCollider)
			{
				base.ResizeCollider();
			}
		}
	}

	public int defaultFontSize
	{
		get
		{
			return (!(this.trueTypeFont != null)) ? ((!(this.mFont != null)) ? 16 : this.mFont.defaultSize) : this.mFontSize;
		}
	}

	public int fontSize
	{
		get
		{
			return this.mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);
			if (this.mFontSize != value)
			{
				this.mFontSize = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			return this.mFontStyle;
		}
		set
		{
			if (this.mFontStyle != value)
			{
				this.mFontStyle = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	public NGUIText.Alignment alignment
	{
		get
		{
			return this.mAlignment;
		}
		set
		{
			if (this.mAlignment != value)
			{
				this.mAlignment = value;
				this.shouldBeProcessed = true;
				this.ProcessAndRequest();
			}
		}
	}

	public bool applyGradient
	{
		get
		{
			return this.mApplyGradient;
		}
		set
		{
			if (this.mApplyGradient != value)
			{
				this.mApplyGradient = value;
				this.MarkAsChanged();
			}
		}
	}

	public Color gradientTop
	{
		get
		{
			return this.mGradientTop;
		}
		set
		{
			if (this.mGradientTop != value)
			{
				this.mGradientTop = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public Color gradientBottom
	{
		get
		{
			return this.mGradientBottom;
		}
		set
		{
			if (this.mGradientBottom != value)
			{
				this.mGradientBottom = value;
				if (this.mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public int spacingX
	{
		get
		{
			return this.mSpacingX;
		}
		set
		{
			if (this.mSpacingX != value)
			{
				this.mSpacingX = value;
				this.MarkAsChanged();
			}
		}
	}

	public int spacingY
	{
		get
		{
			return this.mSpacingY;
		}
		set
		{
			if (this.mSpacingY != value)
			{
				this.mSpacingY = value;
				this.MarkAsChanged();
			}
		}
	}

	private bool keepCrisp
	{
		get
		{
			return this.trueTypeFont != null && this.keepCrispWhenShrunk != UILabel.Crispness.Never && this.keepCrispWhenShrunk == UILabel.Crispness.Always;
		}
	}

	public bool supportEncoding
	{
		get
		{
			return this.mEncoding;
		}
		set
		{
			if (this.mEncoding != value)
			{
				this.mEncoding = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	public NGUIText.SymbolStyle symbolStyle
	{
		get
		{
			return this.mSymbols;
		}
		set
		{
			if (this.mSymbols != value)
			{
				this.mSymbols = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	public UILabel.Overflow overflowMethod
	{
		get
		{
			return this.mOverflow;
		}
		set
		{
			if (this.mOverflow != value)
			{
				this.mOverflow = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return base.width;
		}
		set
		{
			base.width = value;
		}
	}

	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return base.height;
		}
		set
		{
			base.height = value;
		}
	}

	public bool multiLine
	{
		get
		{
			return this.mMaxLineCount != 1;
		}
		set
		{
			if (this.mMaxLineCount != 1 != value)
			{
				this.mMaxLineCount = ((!value) ? 1 : 0);
				this.shouldBeProcessed = true;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return base.localCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return base.worldCorners;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return base.drawingDimensions;
		}
	}

	public int maxLineCount
	{
		get
		{
			return this.mMaxLineCount;
		}
		set
		{
			if (this.mMaxLineCount != value)
			{
				this.mMaxLineCount = Mathf.Max(value, 0);
				this.shouldBeProcessed = true;
				if (this.overflowMethod == UILabel.Overflow.ShrinkContent)
				{
					this.MakePixelPerfect();
				}
			}
		}
	}

	public UILabel.Effect effectStyle
	{
		get
		{
			return this.mEffectStyle;
		}
		set
		{
			if (this.mEffectStyle != value)
			{
				this.mEffectStyle = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return this.mEffectColor;
		}
		set
		{
			if (this.mEffectColor != value)
			{
				this.mEffectColor = value;
				if (this.mEffectStyle != UILabel.Effect.None)
				{
					this.shouldBeProcessed = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return this.mEffectDistance;
		}
		set
		{
			if (this.mEffectDistance != value)
			{
				this.mEffectDistance = value;
				this.shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return this.mOverflow == UILabel.Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				this.overflowMethod = UILabel.Overflow.ShrinkContent;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (this.mLastWidth != this.mWidth || this.mLastHeight != this.mHeight)
			{
				this.mLastWidth = this.mWidth;
				this.mLastHeight = this.mHeight;
				this.mShouldBeProcessed = true;
			}
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return this.mProcessedText;
		}
	}

	public Vector2 printedSize
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return this.mCalculatedSize;
		}
	}

	public override Vector2 localSize
	{
		get
		{
			if (this.shouldBeProcessed)
			{
				this.ProcessText();
			}
			return base.localSize;
		}
	}

	private bool isValid
	{
		get
		{
			return this.mFont != null || this.mTrueTypeFont != null;
		}
	}

	public void ChangeFont()
	{
		if (this.bitmapFont == null || Config.NGUIFont == null)
		{
			return;
		}
		if (this.bitmapFont == Config.NGUIFont)
		{
			this.bitmapFont = Config.SnailFont;
		}
	}

	public static string ForceBR(string strContext)
	{
		if (strContext == null)
		{
			return string.Empty;
		}
		string[] array = strContext.Split(UILabel.separater, StringSplitOptions.None);
		if (array != null)
		{
			UILabel.sb.Remove(0, UILabel.sb.Length);
			int num = array.Length;
			for (int i = 0; i < num - 1; i++)
			{
				UILabel.sb.Append(array[i]);
				UILabel.sb.Append("\n");
			}
			UILabel.sb.Append(array[num - 1]);
			return UILabel.sb.ToString();
		}
		return strContext;
	}

	protected override void OnInit()
	{
		base.OnInit();
		UILabel.mList.Add(this);
		this.SetActiveFont(this.trueTypeFont);
	}

	public override bool CheckLoadAtlas(OnUIWidgetAtlasLoaded onLoaded, UIWidget[] spList, OnUIWidgetAtlasAllLoaded onAllLoaded, GameObject oGo, params object[] args)
	{
		if (this.bitmapFont == null)
		{
			return false;
		}
		if (this.bitmapFont.atlas != null && this.bitmapFont.atlas.spriteMaterial == null)
		{
			this.monLoaded = onLoaded;
			this.mspList = spList;
			this.monAllLoaded = onAllLoaded;
			this.moGo = oGo;
			this.margs = args;
			this.LoadAtlas(this.bitmapFont.atlas.name);
			return true;
		}
		return false;
	}

	public override bool CheckWaitLoadingAtlas()
	{
		return !(this.bitmapFont == null) && this.bitmapFont.atlas != null && this.bitmapFont.atlas.spriteMaterial == null;
	}

	public static void SetLoadAssetCall(UILabel.CallLoadAsset call)
	{
		UILabel.monLoadAsset = call;
	}

	private void LoadAtlas(string strAtlasName)
	{
		if (Application.isPlaying)
		{
			string text = "Parts/Atlas/" + strAtlasName;
			if (UILabel.monLoadAsset != null)
			{
				UILabel.monLoadAsset(text, new AssetCallBack(this.OnFileLoaded));
			}
			else
			{
				UnityEngine.Object @object = Resources.Load(text);
				this.OnFileLoaded(new object[]
				{
					@object
				});
			}
		}
	}

	private void OnFileLoaded(params object[] args)
	{
		AtlasAsset atlasAsset = args[0] as AtlasAsset;
		if (atlasAsset == null)
		{
			if (this.monLoaded != null)
			{
				this.monLoaded(this.mspList, this.monAllLoaded, this.moGo, this.margs);
				this.mspList = null;
				this.moGo = null;
				this.monLoaded = null;
				this.monAllLoaded = null;
				this.margs = null;
			}
			return;
		}
		if (this.bitmapFont.atlas != null)
		{
			this.bitmapFont.atlas.spriteMaterial = atlasAsset.material;
			this.bitmapFont.atlas.MarkAsChanged();
		}
		if (this.monLoaded != null)
		{
			this.monLoaded(this.mspList, this.monAllLoaded, this.moGo, this.margs);
			this.mspList = null;
			this.moGo = null;
			this.monLoaded = null;
			this.monAllLoaded = null;
			this.margs = null;
		}
	}

	protected override void OnDisable()
	{
		this.SetActiveFont(null);
		UILabel.mList.Remove(this);
		base.OnDisable();
	}

	protected void SetActiveFont(Font fnt)
	{
		if (this.mActiveTTF != fnt)
		{
			if (this.mActiveTTF != null)
			{
				int num;
				if (UILabel.mFontUsage.TryGetValue(this.mActiveTTF, out num))
				{
					num = Mathf.Max(0, --num);
					if (num == 0)
					{
						Font.textureRebuilt -= new Action<Font>(UILabel.OnFontTextureChanged);
						UILabel.mFontUsage.Remove(this.mActiveTTF);
					}
					else
					{
						UILabel.mFontUsage[this.mActiveTTF] = num;
					}
				}
				else
				{
					Font.textureRebuilt -= new Action<Font>(UILabel.OnFontTextureChanged);
				}
			}
			this.mActiveTTF = fnt;
			if (this.mActiveTTF != null)
			{
				int num2 = 0;
				if (!UILabel.mFontUsage.TryGetValue(this.mActiveTTF, out num2))
				{
					Font.textureRebuilt += new Action<Font>(UILabel.OnFontTextureChanged);
				}
				num2 = (UILabel.mFontUsage[this.mActiveTTF] = num2 + 1);
			}
		}
	}

	private static void OnFontTextureChanged(Font font)
	{
		for (int i = 0; i < UILabel.mList.size; i++)
		{
			UILabel uILabel = UILabel.mList[i];
			if (uILabel != null)
			{
				Font trueTypeFont = uILabel.trueTypeFont;
				if (trueTypeFont == font)
				{
					trueTypeFont.RequestCharactersInTexture(uILabel.mText, uILabel.mPrintedSize, uILabel.mFontStyle);
					uILabel.MarkAsChanged();
				}
			}
		}
		for (int j = 0; j < UILabel.mList.size; j++)
		{
			UILabel uILabel2 = UILabel.mList[j];
			if (uILabel2 != null)
			{
				Font trueTypeFont2 = uILabel2.trueTypeFont;
				if (trueTypeFont2 == font)
				{
					uILabel2.RemoveFromPanel();
					uILabel2.CreatePanel();
				}
			}
		}
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (this.shouldBeProcessed)
		{
			this.ProcessText();
		}
		return base.GetSides(relativeTo);
	}

	protected override void UpgradeFrom265()
	{
		this.ProcessText(true, true);
		if (this.mShrinkToFit)
		{
			this.overflowMethod = UILabel.Overflow.ShrinkContent;
			this.mMaxLineCount = 0;
		}
		if (this.mMaxLineWidth != 0)
		{
			base.width = this.mMaxLineWidth;
			this.overflowMethod = ((this.mMaxLineCount <= 0) ? UILabel.Overflow.ShrinkContent : UILabel.Overflow.ResizeHeight);
		}
		else
		{
			this.overflowMethod = UILabel.Overflow.ResizeFreely;
		}
		if (this.mMaxLineHeight != 0)
		{
			base.height = this.mMaxLineHeight;
		}
		if (this.mFont != null)
		{
			int defaultSize = this.mFont.defaultSize;
			if (base.height < defaultSize)
			{
				base.height = defaultSize;
			}
		}
		this.mMaxLineWidth = 0;
		this.mMaxLineHeight = 0;
		this.mShrinkToFit = false;
		if (base.GetComponent<BoxCollider>() != null)
		{
			NGUITools.AddWidgetCollider(base.gameObject, true);
		}
	}

	protected override void OnAnchor()
	{
		if (this.mOverflow == UILabel.Overflow.ResizeFreely)
		{
			if (base.isFullyAnchored)
			{
				this.mOverflow = UILabel.Overflow.ShrinkContent;
			}
		}
		else if (this.mOverflow == UILabel.Overflow.ResizeHeight && this.topAnchor.target != null && this.bottomAnchor.target != null)
		{
			this.mOverflow = UILabel.Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	private void ProcessAndRequest()
	{
		if (this.ambigiousFont != null)
		{
			this.ProcessText();
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		this.shouldBeProcessed = true;
		if (this.mLineWidth > 0f)
		{
			this.mMaxLineWidth = Mathf.RoundToInt(this.mLineWidth);
			this.mLineWidth = 0f;
		}
		if (!this.mMultiline)
		{
			this.mMaxLineCount = 1;
			this.mMultiline = true;
		}
		this.mPremultiply = (this.material != null && this.material.shader != null && this.material.shader.name.Contains("Premultiplied"));
		this.ProcessAndRequest();
	}

	public override void MarkAsChanged()
	{
		this.shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		this.ProcessText(false, true);
	}

	private void ProcessText(bool legacyMode, bool full)
	{
		if (!this.isValid)
		{
			return;
		}
		this.mChanged = true;
		this.shouldBeProcessed = false;
		NGUIText.rectWidth = ((!legacyMode) ? base.width : ((this.mMaxLineWidth == 0) ? 1000000 : this.mMaxLineWidth));
		NGUIText.rectHeight = ((!legacyMode) ? base.height : ((this.mMaxLineHeight == 0) ? 1000000 : this.mMaxLineHeight));
		this.mPrintedSize = Mathf.Abs((!legacyMode) ? this.defaultFontSize : Mathf.RoundToInt(base.cachedTransform.localScale.x));
		this.mScale = 1f;
		if (NGUIText.rectWidth < 1 || NGUIText.rectHeight < 0)
		{
			this.mProcessedText = string.Empty;
			return;
		}
		bool flag = this.trueTypeFont != null;
		if (flag && this.keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				this.mDensity = ((!(root != null)) ? 1f : root.pixelSizeAdjustment);
			}
		}
		else
		{
			this.mDensity = 1f;
		}
		if (full)
		{
			this.UpdateNGUIText();
		}
		if (this.mOverflow == UILabel.Overflow.ResizeFreely)
		{
			NGUIText.rectWidth = 1000000;
		}
		if (this.mOverflow == UILabel.Overflow.ResizeFreely || this.mOverflow == UILabel.Overflow.ResizeHeight)
		{
			NGUIText.rectHeight = 1000000;
		}
		if (this.mPrintedSize > 0)
		{
			bool keepCrisp = this.keepCrisp;
			for (int i = this.mPrintedSize; i > 0; i--)
			{
				if (keepCrisp)
				{
					this.mPrintedSize = i;
					NGUIText.fontSize = this.mPrintedSize;
				}
				else
				{
					this.mScale = (float)i / (float)this.mPrintedSize;
					NGUIText.fontScale = ((!flag) ? ((float)this.mFontSize / (float)this.mFont.defaultSize * this.mScale) : this.mScale);
				}
				NGUIText.Update(false);
				if (this.mOriginalText != string.Empty)
				{
					if (UILabel.moQuery != null)
					{
						this.mText = UILabel.moQuery(this.mOriginalText);
					}
					else
					{
						this.mText = this.mOriginalText;
					}
				}
				bool flag2;
				if (this.mIntConvert && UILabel.intConvertFunc != null)
				{
					if (string.IsNullOrEmpty(this.mText) || string.IsNullOrEmpty(this.mText.Trim()))
					{
						flag2 = NGUIText.CustomWrapText(this.mText, out this.mProcessedText);
					}
					else
					{
						flag2 = NGUIText.CustomWrapText(UILabel.intConvertFunc(this.mText), out this.mProcessedText);
					}
				}
				else
				{
					flag2 = NGUIText.CustomWrapText(this.mText, out this.mProcessedText);
				}
				if (this.mOverflow != UILabel.Overflow.ShrinkContent || flag2)
				{
					if (this.mOverflow == UILabel.Overflow.ResizeFreely)
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
						this.mWidth = Mathf.Max(this.minWidth, Mathf.RoundToInt(this.mCalculatedSize.x));
						this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
						if ((this.mWidth & 1) == 1)
						{
							this.mWidth++;
						}
						if ((this.mHeight & 1) == 1)
						{
							this.mHeight++;
						}
					}
					else if (this.mOverflow == UILabel.Overflow.ResizeHeight)
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
						this.mHeight = Mathf.Max(this.minHeight, Mathf.RoundToInt(this.mCalculatedSize.y));
						if ((this.mHeight & 1) == 1)
						{
							this.mHeight++;
						}
					}
					else
					{
						this.mCalculatedSize = NGUIText.CalculatePrintedSize(this.mProcessedText);
					}
					if (legacyMode)
					{
						base.width = Mathf.RoundToInt(this.mCalculatedSize.x);
						base.height = Mathf.RoundToInt(this.mCalculatedSize.y);
						base.cachedTransform.localScale = Vector3.one;
					}
					break;
				}
				if (--i <= 1)
				{
					break;
				}
			}
		}
		else
		{
			base.cachedTransform.localScale = Vector3.one;
			this.mProcessedText = string.Empty;
			this.mScale = 1f;
		}
	}

	public static string ConvertNumber(long value)
	{
		long num = 100000000L;
		long num2 = 10000L;
		if (value >= num)
		{
			return UILabel.StringBuilder(new object[]
			{
				value / num,
				UILabel.GetUIString("UI230114")
			});
		}
		if (value >= num2)
		{
			return UILabel.StringBuilder(new object[]
			{
				value / num2 + UILabel.GetUIString("UI230113")
			});
		}
		return value.ToString();
	}

	public static string StringBuilder(params object[] args)
	{
		UILabel.mstrbuilder.Remove(0, UILabel.mstrbuilder.Length);
		if (args != null)
		{
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				UILabel.mstrbuilder.Append(args[i]);
			}
		}
		return UILabel.mstrbuilder.ToString();
	}

	public static void SetQueryString(UILabel.OnQueryString onQueryString)
	{
		UILabel.moQuery = onQueryString;
	}

	public static string GetUIString(string strTxt)
	{
		if (UILabel.moQuery != null)
		{
			return UILabel.moQuery(strTxt);
		}
		return string.Empty;
	}

	public override void MakePixelPerfect()
	{
		if (this.ambigiousFont != null)
		{
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = (float)Mathf.RoundToInt(localPosition.x);
			localPosition.y = (float)Mathf.RoundToInt(localPosition.y);
			localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = Vector3.one;
			if (this.mOverflow == UILabel.Overflow.ResizeFreely)
			{
				this.AssumeNaturalSize();
			}
			else
			{
				int width = base.width;
				int height = base.height;
				UILabel.Overflow overflow = this.mOverflow;
				this.mOverflow = UILabel.Overflow.ShrinkContent;
				this.mWidth = 100000;
				this.mHeight = 100000;
				this.ProcessText(false, true);
				this.mOverflow = overflow;
				int num = Mathf.RoundToInt(this.mCalculatedSize.x);
				int num2 = Mathf.RoundToInt(this.mCalculatedSize.y);
				num = Mathf.Max(num, base.minWidth);
				num2 = Mathf.Max(num2, base.minHeight);
				this.mWidth = Mathf.Max(width, num);
				this.mHeight = Mathf.Max(height, num2);
				this.MarkAsChanged();
			}
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	public void AssumeNaturalSize()
	{
		if (this.ambigiousFont != null)
		{
			this.mWidth = 100000;
			this.mHeight = 100000;
			this.ProcessText(false, true);
			this.mWidth = Mathf.RoundToInt(this.mCalculatedSize.x);
			this.mHeight = Mathf.RoundToInt(this.mCalculatedSize.y);
			if ((this.mWidth & 1) == 1)
			{
				this.mWidth++;
			}
			if ((this.mHeight & 1) == 1)
			{
				this.mHeight++;
			}
			this.MarkAsChanged();
		}
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector3 worldPos)
	{
		return this.GetCharacterIndexAtPosition(worldPos);
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector2 localPos)
	{
		return this.GetCharacterIndexAtPosition(localPos);
	}

	public int GetCharacterIndexAtPosition(Vector3 worldPos)
	{
		Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
		return this.GetCharacterIndexAtPosition(localPos);
	}

	public int GetCharacterIndexAtPosition(Vector2 localPos)
	{
		if (this.isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			this.UpdateNGUIText();
			NGUIText.PrintCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
			if (UILabel.mTempVerts.size > 0)
			{
				this.ApplyOffset(UILabel.mTempVerts, 0);
				int num = NGUIText.GetClosestCharacter(UILabel.mTempVerts, localPos);
				num = UILabel.mTempIndices[num];
				UILabel.mTempVerts.Clear();
				UILabel.mTempIndices.Clear();
				return num;
			}
		}
		return 0;
	}

	public string GetWordAtPosition(Vector3 worldPos)
	{
		return this.GetWordAtCharacterIndex(this.GetCharacterIndexAtPosition(worldPos));
	}

	public string GetWordAtPosition(Vector2 localPos)
	{
		return this.GetWordAtCharacterIndex(this.GetCharacterIndexAtPosition(localPos));
	}

	public string GetWordAtCharacterIndex(int characterIndex)
	{
		if (characterIndex != -1 && characterIndex < this.mText.Length)
		{
			int num = this.mText.LastIndexOf(' ', characterIndex) + 1;
			int num2 = this.mText.IndexOf(' ', characterIndex);
			if (num2 == -1)
			{
				num2 = this.mText.Length;
			}
			if (num != num2)
			{
				string text = this.mText.Substring(num, num2 - num);
				return NGUIText.StripSymbols(text);
			}
		}
		return null;
	}

	public string GetUrlAtPosition(Vector3 worldPos)
	{
		return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(worldPos));
	}

	public string GetUrlAtPosition(Vector2 localPos)
	{
		return this.GetUrlAtCharacterIndex(this.GetCharacterIndexAtPosition(localPos));
	}

	public string GetUrlAtCharacterIndex(int characterIndex)
	{
		if (characterIndex != -1 && characterIndex < this.mText.Length)
		{
			int num = this.mText.LastIndexOf("[url=", characterIndex);
			if (num != -1)
			{
				num += 5;
				int num2 = this.mText.IndexOf("]", num);
				if (num2 != -1)
				{
					int num3 = this.mText.IndexOf("[/url]", num2);
					if (num3 == -1 || num3 >= characterIndex)
					{
						return this.mText.Substring(num, num2 - num);
					}
				}
			}
		}
		return null;
	}

	public int GetCharacterIndex(int currentIndex, KeyCode key)
	{
		if (this.isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			int defaultFontSize = this.defaultFontSize;
			this.UpdateNGUIText();
			NGUIText.PrintCharacterPositions(processedText, UILabel.mTempVerts, UILabel.mTempIndices);
			if (UILabel.mTempVerts.size > 0)
			{
				this.ApplyOffset(UILabel.mTempVerts, 0);
				int i = 0;
				while (i < UILabel.mTempIndices.size)
				{
					if (UILabel.mTempIndices[i] == currentIndex)
					{
						Vector2 pos = UILabel.mTempVerts[i];
						if (key == KeyCode.UpArrow)
						{
							pos.y += (float)(defaultFontSize + this.spacingY);
						}
						else if (key == KeyCode.DownArrow)
						{
							pos.y -= (float)(defaultFontSize + this.spacingY);
						}
						else if (key == KeyCode.Home)
						{
							pos.x -= 1000f;
						}
						else if (key == KeyCode.End)
						{
							pos.x += 1000f;
						}
						int num = NGUIText.GetClosestCharacter(UILabel.mTempVerts, pos);
						num = UILabel.mTempIndices[num];
						if (num == currentIndex)
						{
							break;
						}
						UILabel.mTempVerts.Clear();
						UILabel.mTempIndices.Clear();
						return num;
					}
					else
					{
						i++;
					}
				}
				UILabel.mTempVerts.Clear();
				UILabel.mTempIndices.Clear();
			}
			if (key == KeyCode.UpArrow || key == KeyCode.Home)
			{
				return 0;
			}
			if (key == KeyCode.DownArrow || key == KeyCode.End)
			{
				return processedText.Length;
			}
		}
		return currentIndex;
	}

	public void PrintOverlay(int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		if (caret != null)
		{
			caret.Clear();
		}
		if (highlight != null)
		{
			highlight.Clear();
		}
		if (!this.isValid)
		{
			return;
		}
		string processedText = this.processedText;
		this.UpdateNGUIText();
		int size = caret.verts.size;
		Vector2 item;
		item.x = 0.5f;
		item.y = 0.5f;
		float finalAlpha = this.finalAlpha;
		if (highlight != null && start != end)
		{
			int size2 = highlight.verts.size;
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, highlight.verts);
			if (highlight.verts.size > size2)
			{
				this.ApplyOffset(highlight.verts, size2);
				Color32 item2;
				item2.r = (byte)highlightColor.r;
				item2.g = (byte)highlightColor.g;
				item2.b = (byte)highlightColor.b;
				item2.a = (byte)(highlightColor.a * finalAlpha);
				for (int i = size2; i < highlight.verts.size; i++)
				{
					highlight.uvs.Add(item);
					highlight.cols.Add(item2);
				}
			}
		}
		else
		{
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, null);
		}
		this.ApplyOffset(caret.verts, size);
		Color32 item3 = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * finalAlpha);
		item3.r = (byte)caretColor.r;
		item3.g = (byte)caretColor.g;
		item3.b = (byte)caretColor.b;
		item3.a = (byte)(caretColor.a * finalAlpha);
		for (int j = size; j < caret.verts.size; j++)
		{
			caret.uvs.Add(item);
			caret.cols.Add(item3);
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!this.isValid)
		{
			return;
		}
		int start = verts.size;
		Color color = base.color;
		color.a = this.finalAlpha;
		if (this.mFont != null && this.mFont.premultipliedAlphaShader)
		{
			color = NGUITools.ApplyPMA(color);
		}
		string processedText = this.processedText;
		int size = verts.size;
		this.UpdateNGUIText();
		NGUIText.tint = color;
		NGUIText.Print(processedText, verts, uvs, cols);
		Vector2 vector = this.ApplyOffset(verts, size);
		if (this.mFont != null && this.mFont.packedFontShader)
		{
			return;
		}
		if (this.effectStyle != UILabel.Effect.None)
		{
			int size2 = verts.size;
			vector.x = this.mEffectDistance.x;
			vector.y = this.mEffectDistance.y;
			this.ApplyShadow(verts, uvs, cols, start, size2, vector.x, -vector.y);
			if (this.effectStyle == UILabel.Effect.Outline)
			{
				start = size2;
				size2 = verts.size;
				this.ApplyShadow(verts, uvs, cols, start, size2, -vector.x, vector.y);
				start = size2;
				size2 = verts.size;
				this.ApplyShadow(verts, uvs, cols, start, size2, vector.x, vector.y);
				start = size2;
				size2 = verts.size;
				this.ApplyShadow(verts, uvs, cols, start, size2, -vector.x, -vector.y);
			}
		}
		if (this.m_symbolOffset != null)
		{
			this.m_symbolOffset();
		}
	}

	public void SetSymbolOffset(Action call)
	{
		this.m_symbolOffset = call;
	}

	protected Vector2 ApplyOffset(BetterList<Vector3> verts, int start)
	{
		Vector2 pivotOffset = base.pivotOffset;
		float num = Mathf.Lerp(0f, (float)(-(float)this.mWidth), pivotOffset.x);
		float num2 = Mathf.Lerp((float)this.mHeight, 0f, pivotOffset.y) + Mathf.Lerp(this.mCalculatedSize.y - (float)this.mHeight, 0f, pivotOffset.y);
		num = Mathf.Round(num);
		num2 = Mathf.Round(num2);
		for (int i = start; i < verts.size; i++)
		{
			Vector3[] expr_80_cp_0 = verts.buffer;
			int expr_80_cp_1 = i;
			expr_80_cp_0[expr_80_cp_1].x = expr_80_cp_0[expr_80_cp_1].x + num;
			Vector3[] expr_99_cp_0 = verts.buffer;
			int expr_99_cp_1 = i;
			expr_99_cp_0[expr_99_cp_1].y = expr_99_cp_0[expr_99_cp_1].y + num2;
		}
		Vector2 zero = Vector2.zero;
		zero.x = num;
		zero.y = num2;
		return zero;
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		Color color = this.mEffectColor;
		color.a *= this.finalAlpha;
		Color32 color2 = (!(this.bitmapFont != null) || !this.bitmapFont.premultipliedAlphaShader) ? color : NGUITools.ApplyPMA(color);
		for (int i = start; i < end; i++)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			Vector3 vector = verts.buffer[i];
			vector.x += x;
			vector.y += y;
			verts.buffer[i] = vector;
			cols.buffer[i] = color2;
		}
	}

	public int CalculateOffsetToFit(string text)
	{
		this.UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
		return NGUIText.CalculateOffsetToFit(text);
	}

	public void SetCurrentProgress()
	{
		if (UIProgressBar.current != null)
		{
			this.text = UIProgressBar.current.value.ToString("F");
		}
	}

	public void SetCurrentPercent()
	{
		if (UIProgressBar.current != null)
		{
			this.text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
		}
	}

	public void SetCustomProgress()
	{
		if (UIProgressBar.current != null)
		{
			float num = UIProgressBar.current.value;
			if (num <= 0.1f)
			{
				num = 0.1f;
			}
			else if ((double)num >= 0.9)
			{
				num = 0.9f;
			}
			this.text = Mathf.RoundToInt(num * 100f) + string.Empty;
		}
	}

	public void SetCustomVoiceProgress()
	{
		if (UIProgressBar.current != null)
		{
			float value = UIProgressBar.current.value;
			UIProgressBar.current.value = (float)Math.Round((double)value, 1);
			this.text = Math.Round((double)value, 1) * 100.0 + string.Empty;
		}
	}

	public void SetCustomScreenCount()
	{
		if (UIProgressBar.current != null)
		{
			float value = UIProgressBar.current.value;
			int num = Mathf.RoundToInt(value * 50f);
			if (num < 10)
			{
				num = 10;
			}
			else if (num > 50)
			{
				num = 50;
			}
			this.text = num + string.Empty;
		}
	}

	public void SetCurrentSelection()
	{
		if (UIPopupList.current != null)
		{
			this.text = ((!UIPopupList.current.isLocalized) ? UIPopupList.current.value : Localization.Get(UIPopupList.current.value));
		}
	}

	public bool Wrap(string text, out string final)
	{
		return this.Wrap(text, out final, 1000000);
	}

	public bool Wrap(string text, out string final, int height)
	{
		this.UpdateNGUIText();
		return NGUIText.WrapText(text, out final);
	}

	public void UpdateNGUIText()
	{
		Font trueTypeFont = this.trueTypeFont;
		bool flag = trueTypeFont != null;
		NGUIText.fontSize = this.mPrintedSize;
		NGUIText.fontStyle = this.mFontStyle;
		NGUIText.rectWidth = this.mWidth;
		NGUIText.rectHeight = this.mHeight;
		NGUIText.gradient = (this.mApplyGradient && (this.mFont == null || !this.mFont.packedFontShader));
		NGUIText.gradientTop = this.mGradientTop;
		NGUIText.gradientBottom = this.mGradientBottom;
		NGUIText.encoding = this.mEncoding;
		NGUIText.premultiply = this.mPremultiply;
		NGUIText.symbolStyle = this.mSymbols;
		NGUIText.maxLines = this.mMaxLineCount;
		NGUIText.spacingX = (float)this.mSpacingX;
		NGUIText.spacingY = (float)this.mSpacingY;
		NGUIText.fontScale = ((!flag) ? ((float)this.mFontSize / (float)this.mFont.defaultSize * this.mScale) : this.mScale);
		if (this.mFont != null)
		{
			NGUIText.bitmapFont = this.mFont;
			while (true)
			{
				UIFont replacement = NGUIText.bitmapFont.replacement;
				if (replacement == null)
				{
					break;
				}
				NGUIText.bitmapFont = replacement;
			}
			if (NGUIText.bitmapFont.isDynamic)
			{
				NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
				NGUIText.bitmapFont = null;
			}
			else
			{
				NGUIText.dynamicFont = null;
			}
		}
		else
		{
			NGUIText.dynamicFont = trueTypeFont;
			NGUIText.bitmapFont = null;
		}
		if (flag && this.keepCrisp)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				NGUIText.pixelDensity = ((!(root != null)) ? 1f : root.pixelSizeAdjustment);
			}
		}
		else
		{
			NGUIText.pixelDensity = 1f;
		}
		if (this.mDensity != NGUIText.pixelDensity)
		{
			this.ProcessText(false, false);
			NGUIText.rectWidth = this.mWidth;
			NGUIText.rectHeight = this.mHeight;
		}
		if (this.alignment == NGUIText.Alignment.Automatic)
		{
			UIWidget.Pivot pivot = base.pivot;
			if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.TopLeft || pivot == UIWidget.Pivot.BottomLeft)
			{
				NGUIText.alignment = NGUIText.Alignment.Left;
			}
			else if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.BottomRight)
			{
				NGUIText.alignment = NGUIText.Alignment.Right;
			}
			else
			{
				NGUIText.alignment = NGUIText.Alignment.Center;
			}
		}
		else
		{
			NGUIText.alignment = this.alignment;
		}
		NGUIText.Update();
	}
}
