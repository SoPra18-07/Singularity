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
        private int mWidth;
        private int mHeight;

        //RenderTargets
        private RenderTarget2D mBloomRenderTarget2DMip0;
        private RenderTarget2D mBloomRenderTarget2DMip1;
        private RenderTarget2D mBloomRenderTarget2DMip2;
        private RenderTarget2D mBloomRenderTarget2DMip3;
        private RenderTarget2D mBloomRenderTarget2DMip4;
        private RenderTarget2D mBloomRenderTarget2DMip5;

        private SurfaceFormat mRenderTargetFormat;

        //Objects
        private GraphicsDevice mGraphicsDevice;
        private QuadRenderer mQuadRenderer;

        //Shader + variables
        private Effect mBloomEffect;

        private EffectPass mBloomPassExtract;
        private EffectPass mBloomPassExtractLuminance;
        private EffectPass mBloomPassDownsample;
        private EffectPass mBloomPassUpsample;
        private EffectPass mBloomPassUpsampleLuminance;

        private EffectParameter mBloomParameterScreenTexture;
        private EffectParameter mBloomInverseResolutionParameter;
        private EffectParameter mBloomRadiusParameter;
        private EffectParameter mBloomStrengthParameter;
        private EffectParameter mBloomStreakLengthParameter;
        private EffectParameter mBloomThresholdParameter;

        //Preset variables for different mip levels
        private float mBloomRadius1 = 1.0f;
        private float mBloomRadius2 = 1.0f;
        private float mBloomRadius3 = 1.0f;
        private float mBloomRadius4 = 1.0f;
        private float mBloomRadius5 = 1.0f;

        private float mBloomStrength1 = 1.0f;
        private float mBloomStrength2 = 1.0f;
        private float mBloomStrength3 = 1.0f;
        private float mBloomStrength4 = 1.0f;
        private float mBloomStrength5 = 1.0f;

        private float BloomStrengthMultiplier = 1.0f;
        
        private float mRadiusMultiplier = 1.0f;


        #endregion

        #region public fields + enums

        private bool BloomUseLuminance = true;
        private int mBloomDownsamplePasses = 5;

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
            get { return mBloomPreset; }
            set
            {
                if (mBloomPreset == value)
                {
                    return;
                }

                mBloomPreset = value;
                SetBloomPreset(mBloomPreset);
            }
        }
        private BloomPresets mBloomPreset;


        private Texture2D BloomScreenTexture { set { mBloomParameterScreenTexture.SetValue(value); } }
        private Vector2 BloomInverseResolution
        {
            get { return mBloomInverseResolutionField; }
            set
            {
                if (value != mBloomInverseResolutionField)
                {
                    mBloomInverseResolutionField = value;
                    mBloomInverseResolutionParameter.SetValue(mBloomInverseResolutionField);
                }
            }
        }
        private Vector2 mBloomInverseResolutionField;

        private float BloomRadius
        {
            get
            {
                return mBloomRadius;
            }

            set
            {
                if (Math.Abs(mBloomRadius - value) > 0.001f)
                {
                    mBloomRadius = value;
                    mBloomRadiusParameter.SetValue(mBloomRadius * mRadiusMultiplier);
                }

            }
        }
        private float mBloomRadius;

        private float BloomStrength
        {
            get { return mBloomStrength; }
            set
            {
                if (Math.Abs(mBloomStrength - value) > 0.001f)
                {
                    mBloomStrength = value;
                    mBloomStrengthParameter.SetValue(mBloomStrength * BloomStrengthMultiplier);
                }

            }
        }
        private float mBloomStrength;

        public float BloomStreakLength
        {
            get { return mBloomStreakLength; }
            set
            {
                if (Math.Abs(mBloomStreakLength - value) > 0.001f)
                {
                    mBloomStreakLength = value;
                    mBloomStreakLengthParameter.SetValue(mBloomStreakLength);
                }
            }
        }
        private float mBloomStreakLength;

        public float BloomThreshold
        {
            get { return mBloomThreshold; }
            set {
                if (Math.Abs(mBloomThreshold - value) > 0.001f)
                {
                    mBloomThreshold = value;
                    mBloomThresholdParameter.SetValue(mBloomThreshold);
                }
            }
        }
        private float mBloomThreshold;

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
            mGraphicsDevice = graphicsDevice;
            UpdateResolution(width, height);
    
            //if quadRenderer == null -> new, otherwise not
            mQuadRenderer = quadRenderer ?? new QuadRenderer(graphicsDevice);

            mRenderTargetFormat = renderTargetFormat;

            //Load the shader parameters and passes for cheap and easy access
            mBloomEffect = content.Load<Effect>("Shaders/BloomFilter/Bloom");
            mBloomInverseResolutionParameter = mBloomEffect.Parameters["InverseResolution"];
            mBloomRadiusParameter = mBloomEffect.Parameters["Radius"];
            mBloomStrengthParameter = mBloomEffect.Parameters["Strength"];
            mBloomStreakLengthParameter = mBloomEffect.Parameters["StreakLength"];
            mBloomThresholdParameter = mBloomEffect.Parameters["Threshold"];

            //For DirectX / Windows
            mBloomParameterScreenTexture = mBloomEffect.Parameters["ScreenTexture"];

            //If we are on OpenGL it's different, load the other one then!
            if (mBloomParameterScreenTexture == null)
            {
                //for OpenGL / CrossPlatform
                mBloomParameterScreenTexture = mBloomEffect.Parameters["LinearSampler+ScreenTexture"];
            }

            mBloomPassExtract = mBloomEffect.Techniques["Extract"].Passes[0];
            mBloomPassExtractLuminance = mBloomEffect.Techniques["ExtractLuminance"].Passes[0];
            mBloomPassDownsample = mBloomEffect.Techniques["Downsample"].Passes[0];
            mBloomPassUpsample = mBloomEffect.Techniques["Upsample"].Passes[0];
            mBloomPassUpsampleLuminance = mBloomEffect.Techniques["UpsampleLuminance"].Passes[0];

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
                        mBloomStrength1 = 0.5f;
                        mBloomStrength2 = 1;
                        mBloomStrength3 = 2;
                        mBloomStrength4 = 1;
                        mBloomStrength5 = 2;
                        mBloomRadius5 = 4.0f;
                        mBloomRadius4 = 4.0f;
                        mBloomRadius3 = 2.0f;
                        mBloomRadius2 = 2.0f;
                        mBloomRadius1 = 1.0f;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 5;
                        break;
                }
                case BloomPresets.SuperWide:
                    {
                        mBloomStrength1 = 0.9f;
                        mBloomStrength2 = 1;
                        mBloomStrength3 = 1;
                        mBloomStrength4 = 2;
                        mBloomStrength5 = 6;
                        mBloomRadius5 = 4.0f;
                        mBloomRadius4 = 2.0f;
                        mBloomRadius3 = 2.0f;
                        mBloomRadius2 = 2.0f;
                        mBloomRadius1 = 2.0f;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Focussed:
                    {
                        mBloomStrength1 = 0.8f;
                        mBloomStrength2 = 1;
                        mBloomStrength3 = 1;
                        mBloomStrength4 = 1;
                        mBloomStrength5 = 2;
                        mBloomRadius5 = 4.0f;
                        mBloomRadius4 = 2.0f;
                        mBloomRadius3 = 2.0f;
                        mBloomRadius2 = 2.0f;
                        mBloomRadius1 = 2.0f;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Small:
                    {
                        mBloomStrength1 = 0.8f;
                        mBloomStrength2 = 1;
                        mBloomStrength3 = 1;
                        mBloomStrength4 = 1;
                        mBloomStrength5 = 1;
                        mBloomRadius5 = 1;
                        mBloomRadius4 = 1;
                        mBloomRadius3 = 1;
                        mBloomRadius2 = 1;
                        mBloomRadius1 = 1;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 5;
                        break;
                    }
                case BloomPresets.Cheap:
                    {
                        mBloomStrength1 = 0.8f;
                        mBloomStrength2 = 2;
                        mBloomRadius2 = 2;
                        mBloomRadius1 = 2;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 2;
                        break;
                    }
                case BloomPresets.One:
                    {
                        mBloomStrength1 = 4f;
                        mBloomStrength2 = 1;
                        mBloomStrength3 = 1;
                        mBloomStrength4 = 1;
                        mBloomStrength5 = 2;
                        mBloomRadius5 = 1.0f;
                        mBloomRadius4 = 1.0f;
                        mBloomRadius3 = 1.0f;
                        mBloomRadius2 = 1.0f;
                        mBloomRadius1 = 1.0f;
                        BloomStreakLength = 1;
                        mBloomDownsamplePasses = 5;
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
            if(mGraphicsDevice==null)
            {
                throw new Exception("Module not yet Loaded / Initialized. Use Load() first");
            }

            //Change renderTarget resolution if different from what we expected. If lower than the inputTexture we gain performance.
            if (width != mWidth || height != mHeight)
            {
                UpdateResolution(width, height);

                //Adjust the blur so it looks consistent across diferrent scalings
                mRadiusMultiplier = (float)width / inputTexture.Width;
                
                //Update our variables with the multiplier
                SetBloomPreset(BloomPreset);
            }

            mGraphicsDevice.RasterizerState = RasterizerState.CullNone;
            mGraphicsDevice.BlendState = BlendState.Opaque;

            //EXTRACT  //Note: Is setRenderTargets(binding better?)
            //We extract the bright values which are above the Threshold and save them to Mip0
            mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip0);

            BloomScreenTexture = inputTexture;
            BloomInverseResolution = new Vector2(1.0f / mWidth, 1.0f / mHeight);
            
            if (BloomUseLuminance)
            {
                mBloomPassExtractLuminance.Apply();
            }
            else
            {
                mBloomPassExtract.Apply();
            }

            mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);
            
            //Now downsample to the next lower mip texture
            if (mBloomDownsamplePasses > 0)
            {
                //DOWNSAMPLE TO MIP1
                mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip1);

                BloomScreenTexture = mBloomRenderTarget2DMip0;
                //Pass
                mBloomPassDownsample.Apply();
                mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                if (mBloomDownsamplePasses > 1)
                {
                    //Our input resolution is halfed, so our inverse 1/res. must be doubled
                    BloomInverseResolution *= 2;

                    //DOWNSAMPLE TO MIP2
                    mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip2);

                    BloomScreenTexture = mBloomRenderTarget2DMip1;
                    //Pass
                    mBloomPassDownsample.Apply();
                    mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                    if (mBloomDownsamplePasses > 2)
                    {
                        BloomInverseResolution *= 2;

                        //DOWNSAMPLE TO MIP3
                        mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip3);

                        BloomScreenTexture = mBloomRenderTarget2DMip2;
                        //Pass
                        mBloomPassDownsample.Apply();
                        mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);
                        
                        if (mBloomDownsamplePasses > 3)
                        {
                            BloomInverseResolution *= 2;

                            //DOWNSAMPLE TO MIP4
                            mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip4);

                            BloomScreenTexture = mBloomRenderTarget2DMip3;
                            //Pass
                            mBloomPassDownsample.Apply();
                            mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                            if (mBloomDownsamplePasses > 4)
                            {
                                BloomInverseResolution *= 2;

                                //DOWNSAMPLE TO MIP5
                                mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip5);

                                BloomScreenTexture = mBloomRenderTarget2DMip4;
                                //Pass
                                mBloomPassDownsample.Apply();
                                mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                                ChangeBlendState();
                                
                                //UPSAMPLE TO MIP4
                                mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip4);
                                BloomScreenTexture = mBloomRenderTarget2DMip5;

                                BloomStrength = mBloomStrength5;
                                BloomRadius = mBloomRadius5;
                                if (BloomUseLuminance)
                                {
                                    mBloomPassUpsampleLuminance.Apply();
                                }
                                else
                                {
                                    mBloomPassUpsample.Apply();
                                }

                                mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                                BloomInverseResolution /= 2;
                            }
                            
                            ChangeBlendState();

                            //UPSAMPLE TO MIP3
                            mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip3);
                            BloomScreenTexture = mBloomRenderTarget2DMip4;

                            BloomStrength = mBloomStrength4;
                            BloomRadius = mBloomRadius4;
                            if (BloomUseLuminance)
                            {
                                mBloomPassUpsampleLuminance.Apply();
                            }
                            else
                            {
                                mBloomPassUpsample.Apply();
                            }

                            mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                            BloomInverseResolution /= 2;

                        }

                        ChangeBlendState();

                        //UPSAMPLE TO MIP2
                        mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip2);
                        BloomScreenTexture = mBloomRenderTarget2DMip3;

                        BloomStrength = mBloomStrength3;
                        BloomRadius = mBloomRadius3;
                        if (BloomUseLuminance)
                        {
                            mBloomPassUpsampleLuminance.Apply();
                        }
                        else
                        {
                            mBloomPassUpsample.Apply();
                        }

                        mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                        BloomInverseResolution /= 2;

                    }

                    ChangeBlendState();

                    //UPSAMPLE TO MIP1
                    mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip1);
                    BloomScreenTexture = mBloomRenderTarget2DMip2;

                    BloomStrength = mBloomStrength2;
                    BloomRadius = mBloomRadius2;
                    if (BloomUseLuminance)
                    {
                        mBloomPassUpsampleLuminance.Apply();
                    }
                    else
                    {
                        mBloomPassUpsample.Apply();
                    }

                    mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);

                    BloomInverseResolution /= 2;
                }

                ChangeBlendState();

                //UPSAMPLE TO MIP0
                mGraphicsDevice.SetRenderTarget(mBloomRenderTarget2DMip0);
                BloomScreenTexture = mBloomRenderTarget2DMip1;

                BloomStrength = mBloomStrength1;
                BloomRadius = mBloomRadius1;

                if (BloomUseLuminance)
                {
                    mBloomPassUpsampleLuminance.Apply();
                }
                else
                {
                    mBloomPassUpsample.Apply();
                }

                mQuadRenderer.RenderQuad(mGraphicsDevice, Vector2.One * -1, Vector2.One);
            }

            //Note the final step could be done as a blend to the final texture.
            
            return mBloomRenderTarget2DMip0;
        }

        private void ChangeBlendState()
        {
            mGraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        /// <summary>
        /// Update the InverseResolution of the used rendertargets. This should be the InverseResolution of the processed image
        /// We use SurfaceFormat.Color, but you can use higher precision buffers obviously.
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        public void UpdateResolution(int width, int height)
        {
            mWidth = width;
            mHeight = height;

            if (mBloomRenderTarget2DMip0 != null)
            {
                Dispose();
            }

            mBloomRenderTarget2DMip0 = new RenderTarget2D(mGraphicsDevice,
                (int) (width),
                (int) (height), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            mBloomRenderTarget2DMip1 = new RenderTarget2D(mGraphicsDevice,
                (int) (width/2),
                (int) (height/2), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            mBloomRenderTarget2DMip2 = new RenderTarget2D(mGraphicsDevice,
                (int) (width/4),
                (int) (height/4), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            mBloomRenderTarget2DMip3 = new RenderTarget2D(mGraphicsDevice,
                (int) (width/8),
                (int) (height/8), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            mBloomRenderTarget2DMip4 = new RenderTarget2D(mGraphicsDevice,
                (int) (width/16),
                (int) (height/16), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            mBloomRenderTarget2DMip5 = new RenderTarget2D(mGraphicsDevice,
                (int) (width/32),
                (int) (height/32), false, mRenderTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        /// <summary>
        ///Dispose our RenderTargets. This is not covered by the Garbage Collector so we have to do it manually
        /// </summary>
        public void Dispose()
        {
            mBloomRenderTarget2DMip0.Dispose();
            mBloomRenderTarget2DMip1.Dispose();
            mBloomRenderTarget2DMip2.Dispose();
            mBloomRenderTarget2DMip3.Dispose();
            mBloomRenderTarget2DMip4.Dispose();
            mBloomRenderTarget2DMip5.Dispose();
        }
    }
}
