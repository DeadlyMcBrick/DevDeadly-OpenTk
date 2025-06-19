using System;
using ImGuiNET;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vector3 = OpenTK.Mathematics.Vector3;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace DevDeadly {}

class Item
{
    public string Name;
    public string Description;
    public int BulletsInformation;
}

class InventorySlot
{
    public Item Item;
    public int Quantity;
}

