using Godot;
using System;

public class Global : Node
{
  //Member variables here, example:
  //private int a = 2;
  //private string b = "textvar";

  public Node MainScene { get; set; }

  public override void _Ready()
  {
      Viewport root = GetTree().GetRoot();
      MainScene = root.GetChild(root.GetChildCount() - 1);
  }

  public void GotoScene(Node currentScene, string path)
  {
    //This function will usually be called from a signal callback,
    //or some other function from the running scene.
    //Deleting the current scene at this point might be
    //a bad idea, because it may be inside of a callback or function of it.
    //The worst case will be a crash or unexpected behavior.

    //The way around this is deferring the load to a later time, when
    //it is ensured that no code from the current scene is running:

    CallDeferred(nameof(DeferredGotoScene), currentScene, path);
  }

  public void DeferredGotoScene(Node currentScene, string path)
  {
      //Immediately free the current scene, there is no risk here.
	  	if (currentScene == null) return;
      currentScene.Free();

      //Load a new scene.
      var nextScene = (PackedScene)GD.Load(path);

      //Instance the new scene.
      currentScene = nextScene.Instance();

      //Add it to the active scene, as child of root.
      MainScene.AddChild(currentScene);

  }

  public bool IsNodeExists(Node node)
  {
    if (node == null || node.NativeInstance == IntPtr.Zero || node.IsQueuedForDeletion()) return false;
    return true;
  }

  public void RemoveChildren(Node node)
  {
    foreach (Node child in node.GetChildren())
    {
      RemoveChildByNode(node, child);
    }
  }

  public void RemoveChildByNode(Node node, Node child, bool isForced = true)
  {
    if (isForced)
    {
      Call("_RemoveChildByNode", node, child, isForced);
    } else {
      CallDeferred("_RemoveChildByNode", node, child, isForced);
    }
  }

  public void _RemoveChildByNode(Node node, Node child, bool isForced)
  {
    if (IsNodeExists(node) && IsNodeExists(child) && node.IsAParentOf(child))
    {
      node.RemoveChild(child);

      if (isForced)
      {
        child.CallDeferred("Free");
      } else {
        child.Free();
      }
    } else if (IsNodeExists(child)) {

      if (isForced)
      {
        child.CallDeferred("Free");
      } else {
        child.Free();
      }
    } else {
      GD.Print("You are trying to remove a null child");
    }
  }
}
