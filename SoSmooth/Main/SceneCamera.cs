using System;
using Gdk;
using Gtk;
using OpenTK;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// A camera than can move around in and render a scene. Consists of a 
    /// pivot entity the camera orbits around.
    /// </summary>
    public class SceneCamera
    {
        /// <summary>
        /// How close to the pivot the camera can be zoomed in.
        /// </summary>
        private const float ZOOM_MIN    = 0.001f;
        /// <summary>
        /// How far from the pivot the camera can be zoomed out.
        /// </summary>
        private const float ZOOM_MAX    = 100.0f;
        /// <summary>
        /// The fraction of the zoom distance gained/lost when zooming in/out.
        /// </summary>
        private const float ZOOM_RATIO  = 1.0f / 8.0f;

        /// <summary>
        /// The angle in radians the pivot rotates per key press.
        /// </summary>
        private const float ROTATE_STEP_SIZE = MathHelper.TwoPi / 28;

        /// <summary>
        /// The time in seconds the camera takes to ease to an orientation.
        /// </summary>
        private const float EASE_TIME = 0.125f;

        private static readonly Quaternion FRONT_VIEW   = new Quaternion(0, 0, 0);
        private static readonly Quaternion BACK_VIEW    = new Quaternion(MathHelper.Pi, 0, 0);
        private static readonly Quaternion RIGHT_VIEW   = new Quaternion(MathHelper.PiOver2, 0, 0);
        private static readonly Quaternion LEFT_VIEW    = new Quaternion(-MathHelper.PiOver2, 0, 0);
        private static readonly Quaternion TOP_VIEW     = new Quaternion(0, 0, MathHelper.PiOver2);
        private static readonly Quaternion BOTTOM_VIEW  = new Quaternion(0, 0, -MathHelper.PiOver2);

        private Quaternion m_rotation = Quaternion.Identity;
        private float m_zoom = 10;

        private bool m_easing;
        private float m_easeStartTime;
        private Quaternion m_easeTargetRot;
        private Quaternion m_easeStartRot;
        
        private Transform m_camPivot;
        private Camera m_camera;

        /// <summary>
        /// Creates a new <see cref="SceneWindow"/> instance.
        /// </summary>
        /// <param name="sceneWindow">The scene window this camera will exist in.</param>
        public SceneCamera(SceneWindow sceneWindow)
        {
            m_camPivot = new Entity(sceneWindow.Scene, "CameraPivot").Transform;
            Transform cam = new Entity(sceneWindow.Scene, "Camera").Transform;
            
            cam.SetParent(m_camPivot);

            cam.LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.PiOver2);

            m_camera = new Camera(cam.Entity);
            sceneWindow.Scene.ActiveCamera = m_camera;

            sceneWindow.KeyPressEvent += OnKeyPress;
            sceneWindow.ButtonPressEvent += OnButtonPress;
            sceneWindow.MotionNotifyEvent += OnMotion;
            sceneWindow.ScrollEvent += OnScroll;
            sceneWindow.SceneUpdate += OnUpdateScene;
        }

        /// <summary>
        /// Updates the camera prior to rendering.
        /// </summary>
        private void OnUpdateScene()
        {
            // smoothly blend towards the target orientation if set
            if (m_easing)
            {
                float fac = Utils.Ease((Time.time - m_easeStartTime) / EASE_TIME);
                m_rotation = Quaternion.Slerp(m_easeStartRot, m_easeTargetRot, fac);
                m_easing = fac < 1;
            }
            
            m_camPivot.LocalRotation = m_rotation;
            m_camera.Transform.LocalPosition = -m_zoom * Vector3.UnitY;
            
            // move the clip planes to keep proportion with the camera's distance from the pivot
            m_camera.NearClip = Math.Min(m_zoom * 0.01f, 1);
            m_camera.FarClip = Math.Max(m_zoom * 2, 100);
        }

        private void OnKeyPress(object o, Gtk.KeyPressEventArgs args)
        {
            ModifierType modifierMask = Accelerator.DefaultModMask;

            bool ctrl = (args.Event.State & modifierMask) == ModifierType.ControlMask;
            bool shift = (args.Event.State & modifierMask) == ModifierType.ShiftMask;

            switch (args.Event.Key)
            {
                case Gdk.Key.KP_1: EaseCamera(ctrl ? BACK_VIEW : FRONT_VIEW); break;
                case Gdk.Key.KP_3: EaseCamera(ctrl ? LEFT_VIEW : RIGHT_VIEW); break;
                case Gdk.Key.KP_7: EaseCamera(ctrl ? TOP_VIEW : BOTTOM_VIEW); break;
                case Gdk.Key.KP_4: YawCamera(-ROTATE_STEP_SIZE); break;
                case Gdk.Key.KP_6: YawCamera(ROTATE_STEP_SIZE); break;
                case Gdk.Key.KP_8: PitchCamera(ROTATE_STEP_SIZE); break;
                case Gdk.Key.KP_2: PitchCamera(-ROTATE_STEP_SIZE); break;
            }
            Logger.Debug(args.Event.Key + " " + args.Event.State);
        }

        private void OnButtonPress(object o, ButtonPressEventArgs args)
        {
            Logger.Debug(args.Event.Device.Name);
        }

        private void OnScroll(object o, ScrollEventArgs args)
        {
            float zoomDelta = ZOOM_RATIO * m_zoom;

            switch (args.Event.Direction)
            {
                case ScrollDirection.Up: ZoomCamera(-zoomDelta); break;
                case ScrollDirection.Down: ZoomCamera(zoomDelta); break;
            }
        }

        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
        }

        /// <summary>
        /// Moves the camera in or out from the pivot by some amount.
        /// </summary>
        /// <param name="zoomDelta">The amount to offset the zoom.</param>
        private void ZoomCamera(float zoomDelta)
        {
            m_zoom = MathHelper.Clamp(m_zoom + zoomDelta, ZOOM_MIN, ZOOM_MAX);
        }

        /// <summary>
        /// Yaws the camera by some angle. Cancels any active easing movements.
        /// </summary>
        /// <param name="yawDelta">The angle in radians.</param>
        private void YawCamera(float yawDelta)
        {
            m_easing = false;
            m_rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, yawDelta) * m_rotation;
        }

        /// <summary>
        /// Pitches the camera by some angle. Cancels any active easing movements.
        /// </summary>
        /// <param name="pitchDelta">The angle in radians.</param>
        private void PitchCamera(float pitchDelta)
        {
            m_easing = false;
            m_rotation = Quaternion.FromAxisAngle(m_camPivot.Right, pitchDelta) * m_rotation;
        }

        /// <summary>
        /// Smoothly moves the camera to a target orientation.
        /// </summary>
        /// <param name="rotation">The goal rotation.</param>
        private void EaseCamera(Quaternion rotation)
        {
            if (m_rotation != rotation)
            {
                m_easing = true;
                m_easeStartTime = Time.time;
                m_easeStartRot = m_rotation;
                m_easeTargetRot = rotation;
            }
        }
    }
}
