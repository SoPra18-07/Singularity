using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Libraries;

namespace Libraries
{
    /// <summary>
    ///
    /// Version 1.1, 16. Dez. 2016
    ///
    /// Bloom / Blur, 2016 TheKosmonaut
    ///
    /// High-Quality Bloom filter for high-performance applications
    ///
    /// Based largely on the implementations in Unreal Engine 4 and Call of Duty AW
    /// For more information look for
    /// "Next Generation Post Processing in Call of Duty Advanced Warfare" by Jorge Jimenez
    /// http://www.iryoku.com/downloads/Next-Generation-Post-Processing-in-Call-of-Duty-Advanced-Warfare-v18.pptx
    ///
    /// The idea is to have several rendertargets or one rendertarget with several mip maps
    /// so each mip has half resolution (1/2 width and 1/2 height) of the previous one.
    ///
    /// 32, 16, 8, 4, 2
    ///
    /// In the first step we extract the bright spots from the original image. If not specified otherwise thsi happens in full resolution.
    /// We can do that based on the average RGB value or Luminance and check whether this value is higher than our Threshold.
    ///     BloomUseLuminance = true / false (default is true)
    ///     BloomThreshold = 0.8f;
    ///
    /// Then we downscale this extraction layer to the next mip map.
    /// While doing that we sample several pixels around the origin.
    /// We continue to downsample a few more times, defined in
    ///     mBloomDownsamplePasses = 5 ( default is 5)
    ///
    /// Afterwards we upsample again, but blur in this step, too.
    /// The final output should be a blur with a very large kernel and smooth gradient.
    ///
    /// The output in the draw is only the blurred extracted texture.
    /// It can be drawn on top of / merged with the original image with an additive operation for example.
    ///
    /// If you use ToneMapping you should apply Bloom before that step.
    /// </summary>
    public class BloomFilter : IDisposable
    {
        #region fields & properties

        #region private fields

        //resolution
        private int _mWidth;
        private int _mHeight;

        //RenderTargets
        private RenderTarget2D _mBloomRenderTarget2DMip0;
        private RenderTarget2D _mBloomRenderTarget2DMip1;
        private RenderTarget2D _mBloomRenderTarget2DMip2;
        private RenderTarget2D _mBloomRenderTarget2DMip3;
        private RenderTarget2D _mBloomRenderTarget2DMip4;
        private RenderTarget2D _mBloomRenderTarget2DMip5;

        private SurfaceFormat _mRenderTargetFormat;

        //Objects
        private GraphicsDevice _mGraphicsDevice;
        private QuadRenderer _mQuadRenderer;

        //Shader + variables
        private Effect _mBloomEffect;

        private EffectPass _mBloomPassExtract;
        private EffectPass _mBloomPassExtractLuminance;
        private EffectPass _mBloomPassDownsample;
        private EffectPass _mBloomPassUpsample;
        private EffectPass _mBloomPassUpsampleLuminance;

        private EffectParameter _mBloomParameterScreenTexture;
        private EffectParameter _mBloomInverseResolutionParameter;
        private EffectParameter _mBloomRadiusParameter;
        private EffectParameter _mBloomStrengthParameter;
        private EffectParameter _mBloomStreakLengthParameter;
        private EffectParameter _mBloomThresholdParameter;

        //Preset variables for different mip levels
        private float _mBloomRadius1 = 1.0f;
        private float _mBloomRadius2 = 1.0f;
        private float _mBloomRadius3 = 1.0f;
        private float _mBloomRadius4 = 1.0f;
        private float _mBloomRadius5 = 1.0f;

        private float _mBloomStrength1 = 1.0f;
        private float _mBloomStrength2 = 1.0f;
        private float _mBloomStrength3 = 1.0f;
        private float _mBloomStrength4 = 1.0f;
        private float _mBloomStrength5 = 1.0f;

        private float _bloomStrengthMultiplier = 1.0f;

        private float _mRadiusMultiplier = 1.0f;


        #endregion

        #region public fields + enums

        private bool _bloomUseLuminance = true;
        private int _mBloomDownsamplePasses = 5;

        //enums
        public enum BloomPresets
        {
            Wide,
            Focussed,
            Small,
            SuperWide,
            Cheap,
            One
        };

        #endregion

        #region properties
        public BloomPresets BloomPreset
        {
            get { return _mBloomPreset; }
            set
            {
                if (_mBloomPreset == value)
                {
                    return;
                }

                _mBloomPreset = value;
                SetBloomPreset(_mBloomPreset);
            }
        }
        private BloomPresets _mBloomPreset;


        private Texture2D BloomScreenTexture { set { _mBloomParameterScreenTexture.SetValue(value); } }
        private Vector2 BloomInverseResolution
        {
            get { return _mBloomInverseResolutionField; }
            set
            {
                if (value != _mBloomInverseResolutionField)
                {
                    _mBloomInverseResolutionField = value;
                    _mBloomInverseResolutionParameter.SetValue(_mBloomInverseResolutionField);
                }
            }
        }
        private Vector2 _mBloomInverseResolutionField;

        private float BloomRadius
        {
            get
            {
                return _mBloomRadius;
            }

            set
            {
                if (Math.Abs(_mBloomRadius - value) > 0.001f)
                {
                    _mBloomRadius = value;
                    _mBloomRadiusParameter.SetValue(_mBloomRadius * _mRadiusMultiplier);
                }

            }
        }
        private float _mBloomRadius;

        private float BloomStrength
        {
            get { return _mBloomStrength; }
            set
            {
                if (Math.Abs(_mBloomStrength - value) > 0.001f)
                {
                    _mBloomStrength = value;
                    _mBloomStrengthParameter.SetValue(_mBloomStrength * _bloomStrengthMultiplier);
                }

            }
        }
        private float _mBloomStrength;

        public float BloomStreakLength
        {
            get { return _mBloomStreakLength; }
            set
            {
                if (Math.Abs(_mBloomStreakLength - value) > 0.001f)
                {
                    _mBloomStreakLength = value;
                    _mBloomStreakLengthParameter.SetValue(_mBloomStreakLength);
                }
            }
        }
        private float _mBloomStreakLength;

        public float BloomThreshold
        {
            get { return _mBloomThreshold; }
            set {
                if (Math.Abs(_mBloomThreshold - value) > 0.001f)
                {
                    _mBloomThreshold = value;
                    _mBloomThresholdParameter.SetValue(_mBloomThreshold);
                }
            }
        }
        private float _mBloomThreshold;

        #endregion

        #endregion

        #region initialize

        /// <summary>
        /// Loads all needed components for the BloomEffect. This effect won't work without calling load
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="content"></param>
        /// <param name="width">initial value for creating the rendertargets</param>
        /// <param name="height">initial value for creating the rendertargets</param>
        /// <param name="renderTargetFormat">The intended format for the rendertargets. For normal, non-hdr, applications color or rgba1010102 are fine NOTE: For OpenGL, SurfaceFormat.Color is recommended for non-HDR applications.</param>
        /// <param name="quadRenderer">if you already have quadRenderer you may reuse it here</param>
        public void Load(GraphicsDevice graphicsDevice, ContentManager content, int width, int height, SurfaceFormat renderTargetFormat = SurfaceFormat.Color,  QuadRenderer quadRenderer = null)
        {
            _mGraphicsDevice = graphicsDevice;
            UpdateResolution(width, height);

            //if quadRenderer == null -> new, otherwise not
            _mQuadRenderer = quadRenderer ?? new QuadRenderer(graphicsDevice);

            _mRenderTargetFormat = renderTargetFormat;

            //Load the shader parameters and passes for cheap and easy access
            _mBloomEffect = content.Load<Effect>("Shaders/BloomFilter/Bloom");
            _mBloomInverseResolutionParameter = _mBloomEffect.Parameters["InverseResolution"];
            _mBloomRadiusParameter = _mBloomEffect.Parameters["Radius"];
            _mBloomStrengthParameter = _mBloomEffect.Parameters["Strength"];
            _mBloomStreakLengthParameter = _mBloomEffect.Parameters["StreakLength"];
            _mBloomThresholdParameter = _mBloomEffect.Parameters["Threshold"];

            //For DirectX / Windows
            _mBloomParameterScreenTexture = _mBloomEffect.Parameters["ScreenTexture"];

            //If we are on OpenGL it's different, load the other one then!
            if (_mBloomParameterScreenTexture == null)
            {
                //for OpenGL / CrossPlatform
                _mBloomParameterScreenTexture = _mBloomEffect.Parameters["LinearSampler+ScreenTexture"];
            }

            _mBloomPassExtract = _mBloomEffect.Techniques["Extract"].Passes[0];
            _mBloomPassExtractLuminance = _mBloomEffect.Techniques["ExtractLuminance"].Passes[0];
            _mBloomPassDownsample = _mBloomEffect.Techniques["Downsample"].Passes[0];
            _mBloomPassUpsample = _mBloomEffect.Techniques["Upsample"].Passes[0];
            _mBloomPassUpsampleLuminance = _mBloomEffect.Techniques["UpsampleLuminance"].Passes[0];

            //An interesting blendstate for merging the initial image with the bloom.
            //BlendStateBloom = new BlendState();
            //BlendStateBloom.ColorBlendFunction = BlendFunction.Add;
            //BlendStateBloom.ColorSourceBlend = Blend.BlendFactor;
            //BlendStateBloom.ColorDestinationBlend = Blend.BlendFactor;
            //BlendStateBloom.BlendFactor = new Color(0.5f, 0.5f, 0.5f);

            //Default threshold.
            BloomThreshold = 0.8f;
            //Setup the default preset values.
            //BloomPreset = BloomPresets.One;
            SetBloomPreset(BloomPreset);
        }

        /// <summary>
        /// A few presets with different values for the different mip levels of our bloom.
        /// </summary>
        /// <param name="preset">See BloomPresets enums. Example: BloomPresets.Wide</param>
        private void SetBloomPreset(BloomPresets preset)
        {
            switch(preset)
            {
                case BloomPresets.Wide:
                {
                        _mBloomStrength1 = 0.5f;
                        _mBloomStrength2 = 1;
                        _mBloomStrength3 = 2;
                        _mBloomStrength4 = 1;
                        _mBloomStrength5 = 2;
                        _mBloomRadius5 = 4.0f;
                        _mBloomRadius4 = 4.0f;
                        _mBloomRadius3 = 2.0f;
                        _mBloomRadius2 = 2.0f;
                        _mBloomRadius1 = 1.0f;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 5;
                        break;
                }
                case BloomPresets.SuperWide:
                    {
                        _mBloomStrength1 = 0.9f;
                        _mBloomStrength2 = 1;
                        _mBloomStrength3 = 1;
                        _mBloomStrength4 = 2;
                        _mBloomStrength5 = 6;
                        _mBloomRadius5 = 4.0f;
                        _mBloomRadius4 = 2.0f;
                        _mBloomRadius3 = 2.0f;
                        _mBloomRadius2 = 2.0f;
                        _mBloomRadius1 = 2.0f;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Focussed:
                    {
                        _mBloomStrength1 = 0.8f;
                        _mBloomStrength2 = 1;
                        _mBloomStrength3 = 1;
                        _mBloomStrength4 = 1;
                        _mBloomStrength5 = 2;
                        _mBloomRadius5 = 4.0f;
                        _mBloomRadius4 = 2.0f;
                        _mBloomRadius3 = 2.0f;
                        _mBloomRadius2 = 2.0f;
                        _mBloomRadius1 = 2.0f;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Small:
                    {
                        _mBloomStrength1 = 0.8f;
                        _mBloomStrength2 = 1;
                        _mBloomStrength3 = 1;
                        _mBloomStrength4 = 1;
                        _mBloomStrength5 = 1;
                        _mBloomRadius5 = 1;
                        _mBloomRadius4 = 1;
                        _mBloomRadius3 = 1;
                        _mBloomRadius2 = 1;
                        _mBloomRadius1 = 1;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Cheap:
                    {
                        _mBloomStrength1 = 0.8f;
                        _mBloomStrength2 = 2;
                        _mBloomRadius2 = 2;
                        _mBloomRadius1 = 2;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 2;
                        break;
                    }
                case BloomPresets.One:
                    {
                        _mBloomStrength1 = 4f;
                        _mBloomStrength2 = 1;
                        _mBloomStrength3 = 1;
                        _mBloomStrength4 = 1;
                        _mBloomStrength5 = 2;
                        _mBloomRadius5 = 1.0f;
                        _mBloomRadius4 = 1.0f;
                        _mBloomRadius3 = 1.0f;
                        _mBloomRadius2 = 1.0f;
                        _mBloomRadius1 = 1.0f;
                        BloomStreakLength = 1;
                        _mBloomDownsamplePasses = 5;
                        break;
                    }
            }
        }

        #endregion

        /// <summary>
        /// Main draw function
        /// </summary>
        /// <param name="inputTexture">the image from which we want to extract bright parts and blur these</param>
        /// <param name="width">width of our target. If different to the input.Texture width our final texture will be smaller/larger.
        /// For example we can use half resolution. Input: 1280px wide -> width = 640px
        /// The smaller this value the better performance and the worse our final image quality</param>
        /// <param name="height">see: width</param>
        /// <returns></returns>
        public Texture2D Draw(Texture2D inputTexture, int width, int height)
        {
            //Check if we are initialized
            if(_mGraphicsDevice==null)
            {
                throw new Exception("Module not yet Loaded / Initialized. Use Load() first");
            }

            //Change renderTarget resolution if different from what we expected. If lower than the inputTexture we gain performance.
            if (width != _mWidth || height != _mHeight)
            {
                UpdateResolution(width, height);

                //Adjust the blur so it looks consistent across diferrent scalings
                _mRadiusMultiplier = (float)width / inputTexture.Width;

                //Update our variables with the multiplier
                SetBloomPreset(BloomPreset);
            }

            _mGraphicsDevice.RasterizerState = RasterizerState.CullNone;
            _mGraphicsDevice.BlendState = BlendState.Opaque;

            //EXTRACT  //Note: Is setRenderTargets(binding better?)
            //We extract the bright values which are above the Threshold and save them to Mip0
            _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip0);

            BloomScreenTexture = inputTexture;
            BloomInverseResolution = new Vector2(1.0f / _mWidth, 1.0f / _mHeight);

            if (_bloomUseLuminance)
            {
                _mBloomPassExtractLuminance.Apply();
            }
            else
            {
                _mBloomPassExtract.Apply();
            }

            _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

            //Now downsample to the next lower mip texture
            if (_mBloomDownsamplePasses > 0)
            {
                //DOWNSAMPLE TO MIP1
                _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip1);

                BloomScreenTexture = _mBloomRenderTarget2DMip0;
                //Pass
                _mBloomPassDownsample.Apply();
                _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                if (_mBloomDownsamplePasses > 1)
                {
                    //Our input resolution is halfed, so our inverse 1/res. must be doubled
                    BloomInverseResolution *= 2;

                    //DOWNSAMPLE TO MIP2
                    _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip2);

                    BloomScreenTexture = _mBloomRenderTarget2DMip1;
                    //Pass
                    _mBloomPassDownsample.Apply();
                    _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                    if (_mBloomDownsamplePasses > 2)
                    {
                        BloomInverseResolution *= 2;

                        //DOWNSAMPLE TO MIP3
                        _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip3);

                        BloomScreenTexture = _mBloomRenderTarget2DMip2;
                        //Pass
                        _mBloomPassDownsample.Apply();
                        _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                        if (_mBloomDownsamplePasses > 3)
                        {
                            BloomInverseResolution *= 2;

                            //DOWNSAMPLE TO MIP4
                            _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip4);

                            BloomScreenTexture = _mBloomRenderTarget2DMip3;
                            //Pass
                            _mBloomPassDownsample.Apply();
                            _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                            if (_mBloomDownsamplePasses > 4)
                            {
                                BloomInverseResolution *= 2;

                                //DOWNSAMPLE TO MIP5
                                _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip5);

                                BloomScreenTexture = _mBloomRenderTarget2DMip4;
                                //Pass
                                _mBloomPassDownsample.Apply();
                                _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                                ChangeBlendState();

                                //UPSAMPLE TO MIP4
                                _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip4);
                                BloomScreenTexture = _mBloomRenderTarget2DMip5;

                                BloomStrength = _mBloomStrength5;
                                BloomRadius = _mBloomRadius5;
                                if (_bloomUseLuminance)
                                {
                                    _mBloomPassUpsampleLuminance.Apply();
                                }
                                else
                                {
                                    _mBloomPassUpsample.Apply();
                                }

                                _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                                BloomInverseResolution /= 2;
                            }

                            ChangeBlendState();

                            //UPSAMPLE TO MIP3
                            _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip3);
                            BloomScreenTexture = _mBloomRenderTarget2DMip4;

                            BloomStrength = _mBloomStrength4;
                            BloomRadius = _mBloomRadius4;
                            if (_bloomUseLuminance)
                            {
                                _mBloomPassUpsampleLuminance.Apply();
                            }
                            else
                            {
                                _mBloomPassUpsample.Apply();
                            }

                            _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                            BloomInverseResolution /= 2;

                        }

                        ChangeBlendState();

                        //UPSAMPLE TO MIP2
                        _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip2);
                        BloomScreenTexture = _mBloomRenderTarget2DMip3;

                        BloomStrength = _mBloomStrength3;
                        BloomRadius = _mBloomRadius3;
                        if (_bloomUseLuminance)
                        {
                            _mBloomPassUpsampleLuminance.Apply();
                        }
                        else
                        {
                            _mBloomPassUpsample.Apply();
                        }

                        _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                        BloomInverseResolution /= 2;

                    }

                    ChangeBlendState();

                    //UPSAMPLE TO MIP1
                    _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip1);
                    BloomScreenTexture = _mBloomRenderTarget2DMip2;

                    BloomStrength = _mBloomStrength2;
                    BloomRadius = _mBloomRadius2;
                    if (_bloomUseLuminance)
                    {
                        _mBloomPassUpsampleLuminance.Apply();
                    }
                    else
                    {
                        _mBloomPassUpsample.Apply();
                    }

                    _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);

                    BloomInverseResolution /= 2;
                }

                ChangeBlendState();

                //UPSAMPLE TO MIP0
                _mGraphicsDevice.SetRenderTarget(_mBloomRenderTarget2DMip0);
                BloomScreenTexture = _mBloomRenderTarget2DMip1;

                BloomStrength = _mBloomStrength1;
                BloomRadius = _mBloomRadius1;

                if (_bloomUseLuminance)
                {
                    _mBloomPassUpsampleLuminance.Apply();
                }
                else
                {
                    _mBloomPassUpsample.Apply();
                }

                _mQuadRenderer.RenderQuad(_mGraphicsDevice, Vector2.One * -1, Vector2.One);
            }

            //Note the final step could be done as a blend to the final texture.

            return _mBloomRenderTarget2DMip0;
        }

        private void ChangeBlendState()
        {
            _mGraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        /// <summary>
        /// Update the InverseResolution of the used rendertargets. This should be the InverseResolution of the processed image
        /// We use SurfaceFormat.Color, but you can use higher precision buffers obviously.
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        public void UpdateResolution(int width, int height)
        {
            _mWidth = width;
            _mHeight = height;

            if (_mBloomRenderTarget2DMip0 != null)
            {
                Dispose();
            }

            _mBloomRenderTarget2DMip0 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width),
                (int) (height), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            _mBloomRenderTarget2DMip1 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width/2),
                (int) (height/2), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _mBloomRenderTarget2DMip2 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width/4),
                (int) (height/4), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _mBloomRenderTarget2DMip3 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width/8),
                (int) (height/8), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _mBloomRenderTarget2DMip4 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width/16),
                (int) (height/16), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _mBloomRenderTarget2DMip5 = new RenderTarget2D(_mGraphicsDevice,
                (int) (width/32),
                (int) (height/32), false, _mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        ///Dispose our RenderTargets. This is not covered by the Garbage Collector so we have to do it manually
        /// </summary>
        public void Dispose()
        {
            _mBloomRenderTarget2DMip0.Dispose();
            _mBloomRenderTarget2DMip1.Dispose();
            _mBloomRenderTarget2DMip2.Dispose();
            _mBloomRenderTarget2DMip3.Dispose();
            _mBloomRenderTarget2DMip4.Dispose();
            _mBloomRenderTarget2DMip5.Dispose();
        }
    }
}
