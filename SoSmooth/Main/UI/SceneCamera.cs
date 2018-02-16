﻿using System;
using Gdk;
using Gtk;
using OpenTK;
using SoSmooth.Scenes;

namespace SoSmooth
{
    /// <summary>
    /// A camera than can move around in and render a scene. Consists of a 
    /// pivot entity and a camera that orbits around it.
    /// </summary>
    public class SceneCamera
    {
        /// <summary>
        /// The sensitivity of the mouse drag zoom.
        /// </summary>
        private const float DRAG_ZOOM_SENSITIVITY = 0.0075f;
        /// <summary>
        /// The sensitivity of the mouse drag rotate.
        /// </summary>
        private const float DRAG_ROTATE_SENSITIVITY = 0.008f;
        /// <summary>
        /// The sensitivity of the mouse drag pan.
        /// </summary>
        private const float DRAG_PAN_SENSITIVITY = 1.2f;

        /// <summary>
        /// How close to the pivot the camera can be zoomed in.
        /// </summary>
        private const float ZOOM_MIN    = 0.001f;
        /// <summary>
        /// How far from the pivot the camera can be zoomed out.
        /// </summary>
        private const float ZOOM_MAX    = 100.0f;
        /// <summary>
        /// The fraction of the zoom distance gained/lost when stepping zoom in/out.
        /// </summary>
        private const float ZOOM_RATIO  = 1.0f / 5.0f;

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

        private Transform m_camPivot;
        private Camera m_camera;

        private Vector2d m_lastMousePos;

        private bool m_easing;
        private float m_easeStartTime;
        private Quaternion m_easeTargetRot;
        private Quaternion m_easeStartRot;

        private float m_yaw = 0;
        private float m_pitch = 0;
        private float m_zoom = 10;
        
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
                float fac = MathHelper.Clamp(Utils.Ease((Time.time - m_easeStartTime) / EASE_TIME), 0, 1);
                m_camPivot.LocalRotation = Quaternion.Slerp(m_easeStartRot, m_easeTargetRot, fac);
                m_easing = fac < 1;
            }
            else
            {
                m_camPivot.LocalRotation = GetRotation();
            }
            
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
                case Gdk.Key.KP_8: PitchCamera(-ROTATE_STEP_SIZE); break;
                case Gdk.Key.KP_2: PitchCamera(ROTATE_STEP_SIZE); break;
            }
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

        private void OnButtonPress(object o, ButtonPressEventArgs args)
        {
            // clear the mouse offset so the the first frame of mouse motion is zero.
            m_lastMousePos = new Vector2d(args.Event.X, args.Event.Y);
        }

        private void OnMotion(object o, MotionNotifyEventArgs args)
        {
            ModifierType modifierMask = Accelerator.DefaultModMask;
            
            bool ctrl = (args.Event.State & modifierMask) == ModifierType.ControlMask;
            bool shift = (args.Event.State & modifierMask) == ModifierType.ShiftMask;
            bool middleMouse = (args.Event.State & ModifierType.Button2Mask) != 0;

            if (middleMouse)
            {
                // get the amount the mouse has moved since last event
                Vector2d newMousePos = new Vector2d(args.Event.X, args.Event.Y);
                Vector2d delta = newMousePos - m_lastMousePos;
                m_lastMousePos = newMousePos;

                if (ctrl) // zoom
                {
                    ZoomCamera((float)delta.Y * m_zoom * DRAG_ZOOM_SENSITIVITY);
                }
                else if (shift) // pan
                {
                    // scale sensitivity by the vertical resolution when panning
                    int resX, resY;
                    m_camera.GetResolution(out resX, out resY);
                    float sensitivity = m_zoom * (DRAG_PAN_SENSITIVITY / resY);

                    m_camPivot.LocalPosition += m_camPivot.Right * (float)delta.X * sensitivity;
                    m_camPivot.LocalPosition += m_camPivot.Up * (float)delta.Y * sensitivity;
                }
                else // rotate
                {
                    YawCamera((float)delta.X * -DRAG_ROTATE_SENSITIVITY);
                    PitchCamera((float)delta.Y * -DRAG_ROTATE_SENSITIVITY);
                }
            }
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
            if (!m_easing)
            {
                m_yaw += yawDelta;
            }
        }

        /// <summary>
        /// Pitches the camera by some angle. Cancels any active easing movements.
        /// </summary>
        /// <param name="pitchDelta">The angle in radians.</param>
        private void PitchCamera(float pitchDelta)
        {
            if (!m_easing)
            {
                m_pitch += pitchDelta;
            }
        }

        /// <summary>
        /// Smoothly moves the camera to a target orientation.
        /// </summary>
        /// <param name="targetRotation">The goal rotation.</param>
        private void EaseCamera(Quaternion targetRotation)
        {
            Quaternion currentRotation = GetRotation();

            if (currentRotation != targetRotation)
            {
                m_easing = true;
                m_easeStartTime = Time.time;
                m_easeStartRot = currentRotation;
                m_easeTargetRot = targetRotation;

                Vector3 euler = targetRotation.ToEulerAngles();
                m_yaw = euler.Z;
                m_pitch = euler.X;
            }
        }

        /// <summary>
        /// Computes the pivot rotation using the current yaw and pitch.
        /// </summary>
        private Quaternion GetRotation()
        {
            return Quaternion.FromAxisAngle(Vector3.UnitZ, m_yaw) * Quaternion.FromAxisAngle(Vector3.UnitX, m_pitch);
        }
    }
}