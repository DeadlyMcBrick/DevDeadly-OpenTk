using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DevDeadly
{
    public class Camera
    {
        private float YAW = -90.0f;
        private float SPEED = 16f;
        private float SCREENWIDTH;
        private float SCREENHEIGHT;
        private float SENSITIVITY = 13f;
        private float pitch;
        private bool  firstMove = true;

        public Vector3 position;
        public Vector3 up = Vector3.UnitY;
        public Vector3 front = -Vector3.UnitZ;
        public Vector3 right = Vector3.UnitX;
        public Vector2 lastPos;

        private List<BoundingBox> obstacles = new List<BoundingBox>();

        public Camera(float width, float height, Vector3 position)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + front, up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70.0f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 100.0f);
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

            front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(YAW));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(YAW));
            front = Vector3.Normalize(front);
            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

        public void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            Vector3 newPosition = position;

            if (input.IsKeyDown(Keys.W)) newPosition += front * SPEED * (float)e.Time;
            if (!CheckCollision(newPosition))
            {
                position = newPosition;
            }

            if (input.IsKeyDown(Keys.A)) newPosition -= right * SPEED * (float)e.Time;
            if (!CheckCollision(newPosition))
            {
                position = newPosition;
            }

            if (input.IsKeyDown(Keys.S)) newPosition -= front * SPEED * (float)e.Time;
            if (!CheckCollision(newPosition))
            {
                position = newPosition;
            }

            if (input.IsKeyDown(Keys.D)) newPosition += right * SPEED * (float)e.Time;
            if (!CheckCollision(newPosition))
            {
                position = newPosition;
            }

            if (input.IsKeyDown(Keys.Space))
            {
                position.Y += SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.LeftControl))
            {
                position.Y -= SPEED * (float)e.Time;
            }

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
                YAW += deltaX * SENSITIVITY * (float)e.Time;
                pitch -= deltaY * SENSITIVITY * (float)e.Time;
            }

            UpdateVectors();
        }

        public void SetObstacles(List<BoundingBox> boxes)
        {
            obstacles = boxes;
        }

        private bool CheckCollision(Vector3 newPosition)
        {
            BoundingBox cameraBox = new BoundingBox(
                newPosition - new Vector3(0.25f),
                newPosition + new Vector3(0.25f)
            );

            foreach (var box in obstacles)
            {
                if (cameraBox.Intersects(box))
                {
                    return true;
                }
            }
            return false;
        }

        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }
    }
}
