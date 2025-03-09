using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace DevDeadly
{

    public class Camera
    {
        private float SPEED = 16f;
        private float SCREENWIDTH;
        private float SCREENHEIGHT;
        private float SENSITIVITY = 9f;

        public Vector3 position;

        private Vector3 up = Vector3.UnitY;
        private Vector3 front = -Vector3.UnitZ;
        private Vector3 right = Vector3.UnitX;

        private float pitch;
        private float yaw = -90.0f;

        private bool firstMove = true;
        public Vector2 lastPos;

        private List<BoundingBox> obstacles;

        public Camera(float width, float height, Vector3 position, List<BoundingBox> obstacles)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
            this.obstacles = obstacles;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 100.0f);
        }

        private void UpdateVectors()
        {
            if (pitch > 89.0f)
            {
                pitch = 89.0f;
            }
            if (pitch < -89.0f)
            {
                pitch = -89.0f;
            }

            front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));

            front = Vector3.Normalize(front);

            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

        public void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            Vector3 newPosition = position;

            // Detectar el movimiento
            if (input.IsKeyDown(Keys.W)) newPosition += front * SPEED * (float)e.Time;
            if (input.IsKeyDown(Keys.A)) newPosition -= right * SPEED * (float)e.Time;
            if (input.IsKeyDown(Keys.S)) newPosition -= front * SPEED * (float)e.Time;
            if (input.IsKeyDown(Keys.D)) newPosition += right * SPEED * (float)e.Time;
            if (input.IsKeyDown(Keys.Space)) newPosition.Y += SPEED * (float)e.Time;
            if (input.IsKeyDown(Keys.LeftControl)) newPosition.Y -= SPEED * (float)e.Time;

            // Verificar colisiones antes de mover
            if (!CheckCollision(newPosition))
            {
                position = newPosition;
            }

            // Rotación del mouse
            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                yaw += deltaX * SENSITIVITY * (float)e.Time;
                pitch -= deltaY * SENSITIVITY * (float)e.Time;
            }

            UpdateVectors();
        }

        private bool CheckCollision(Vector3 newPosition)
        {
            // Comprobamos si la nueva posición de la cámara colide con algún cubo (AABB)
            foreach (var box in obstacles)
            {
                if (box.Intersects(newPosition))
                {
                    return true; // Colisión detectada
                }
            }
            return false; // No hay colisión
        }

        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }
    }
}
