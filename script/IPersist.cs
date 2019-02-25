using Godot;
using System;

interface IPersist
{
  Dictionary<object, object> Save();
}
