using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CompositionAnimationsDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Visual orbitVisual;
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Get a reference to the compositor object for the current page
            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Get a reference to the outer orbit visual object
            orbitVisual = ElementCompositionPreview.GetElementVisual(Orbit);

            // Create a new animation using Compositor's factory methods
            var orbitAnimation = compositor.CreateScalarKeyFrameAnimation();
            // This is how long the animation is going to last
            orbitAnimation.Duration = TimeSpan.FromSeconds(10);

            // How many times is the animation going to repeat - infinite loop
            orbitAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

            // At the end of the animation the end value is 360, and we're using a linear easing 
            orbitAnimation.InsertKeyFrame(1f, 360, compositor.CreateLinearEasingFunction());

            // By default the Grid's center will be the top left corner of the page, let's center it
            orbitVisual.CenterPoint = new Vector3((float)Window.Current.Bounds.Width / 2, (float)Window.Current.Bounds.Height / 2, 0);

            // Start our animation
            orbitVisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), orbitAnimation);

            var propertySet = compositor.CreatePropertySet();
            propertySet.InsertScalar("angle", 0);

            var angleAnimation = compositor.CreateScalarKeyFrameAnimation();
            angleAnimation.Duration = TimeSpan.FromSeconds(10);
            angleAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            angleAnimation.InsertKeyFrame(1f, 360, compositor.CreateLinearEasingFunction());
            propertySet.StartAnimation("angle", angleAnimation);

            Visual planetVisual = ElementCompositionPreview.GetElementVisual(Planet);
            planetVisual.TransformMatrix = Matrix4x4.CreateTranslation(new Vector3(100, 0, 0));


            var satelliteVisual = ElementCompositionPreview.GetElementVisual(Satellite);
            // Offset the satellite from the planet
            satelliteVisual.TransformMatrix = Matrix4x4.CreateTranslation(new Vector3(30, 0, 0));
            // Adjust the center point to be exactly in the center of the satellite
            satelliteVisual.CenterPoint = new Vector3((float)Satellite.ActualWidth / 2, (float)Satellite.ActualHeight / 2, 0);

            var satelliteAnimation = compositor.CreateExpressionAnimation();

            // The expression that controls the value of the animatable object
            satelliteAnimation.Expression = "3 * propertySet.angle";
            // Fill in the necessary variables and parameters used in the expression
            satelliteAnimation.SetReferenceParameter("propertySet", propertySet);
            satelliteVisual.StartAnimation(nameof(Visual.RotationAngleInDegrees), satelliteAnimation);

            var starsVisual = ElementCompositionPreview.GetElementVisual(StarField);

            var starsAnimation = compositor.CreateExpressionAnimation();
            starsAnimation.Expression = "Max(0.3, Abs(Cos(propertySet.angle * 0.02)))";
            starsAnimation.SetReferenceParameter("propertySet", propertySet);

            starsVisual.StartAnimation(nameof(Visual.Opacity), starsAnimation);

            SizeChanged += MainPage_SizeChanged;

        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            orbitVisual.CenterPoint = new Vector3((float)Window.Current.Bounds.Width / 2, (float)Window.Current.Bounds.Height / 2, 0);
        }
    }
}
