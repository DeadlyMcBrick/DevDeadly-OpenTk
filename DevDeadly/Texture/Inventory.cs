using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace DevDeadly
{
}

public class TextureHUD
{
    //public int Handle { get; private set; }

   public int Handle3;

    public TextureHUD(string path)
    {
        Handle3 = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, Handle3);
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.NearestMipmapLinear);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        //Multiple Texture

        GL.ActiveTexture(TextureUnit.Texture10);
        GL.BindTexture(TextureTarget.Texture2D, Handle3);
    }

    public void Use(TextureUnit unit = TextureUnit.Texture10)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle3);

    }
}
