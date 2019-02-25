using Godot;
using System;

public class BackgroundLoad : Node
{
    //Member variables here, example:
    //private int a = 2;
    //private string b = "textvar";
	private const int OK = 0;
	private const int ERR_FILE_EOF = 18;
	private const int SIMULATED_DELAY_SEC = 1;	
	
 	private ResourceInteractiveLoader _loader = null;
	private ProgressBar _progress = null;

    public override void _Ready()
    {
		_progress = (ProgressBar)GetNode("ProgressBar");
    }

	//game requests to switch to this scene	
	public void GotoScene(string path) 
	{
		CallDeferred(nameof(DeferredGotoScene), path);
	}	
	
	public void DeferredGotoScene(string path) 
	{ 
	    _loader = ResourceLoader.LoadInteractive(path);

		//check for errors
	    if (_loader == null) 
		{
			GD.Print("ResourceLoader is null and check if the resource path is valid : ", path);
	        return;
		} 
		
		var totalStages = _loader.GetStageCount();
		CallDeferred(nameof(DeferredProgressBarSetMax), totalStages);
		
		SetProcess(true);
		
	   //Raise(); //show on top
		_progress.SetVisible(true);
		_progress.Show();
	}
		
	public override void _Process(float delta)
	{
	    if (_loader == null) //no need to process anymore
		{
	        SetProcess(false);
	        return;
		}
		
		PackedScene pScene = null;
		
	    while (true)  //use "time_max" to control how much time we block this thread
		{	
			var currentStage = _loader.GetStage();
			GD.Print("stage: ", currentStage);
			CallDeferred(nameof(DeferredProgressBarSetValue), currentStage);

			
			//Simulate a delay
			OS.DelayMsec( SIMULATED_DELAY_SEC * 1000);
							
	        var err = (int) _loader.Poll();			
			GD.Print("err: ", err);
	
	        if (err == ERR_FILE_EOF) //load finished
			{
	            pScene = (PackedScene)_loader.GetResource();
	            _loader = null;
	            break;
	        } else if (err != OK) {  //error during loading
				GD.Print("ResourceLoader failed on loading resource!!");
	            _loader = null;
	            break;
			}
		}
		
		CallDeferred("SetNewScene", pScene);
	}
	
	private void DeferredProgressBarSetMax(int total)
	{
		_progress.SetMax(total);
	}
	
	private void DeferredProgressBarSetValue(int val)
	{
		_progress.SetValue(val);
	}
	
	private void SetNewScene(PackedScene pScene)
	{
		//check for errors
	    if (pScene == null) 
		{
			GD.Print("No resource is available.");
	        return;
		}

		GD.Print("This job is done.");

		_progress.Hide();
		
		Node newScene = pScene.Instance();
		
		GetTree().CurrentScene.Free();
		GetTree().CurrentScene = null;
		
		GetTree().Root.AddChild(newScene);
		GetTree().CurrentScene = newScene;

		_progress.SetVisible(false);
	}

	//Note: This can be called from anywhere inside the tree.  This function is path independent.
	public void LoadSavedGame()
	{
	    var saveGame = new File();
	    if (!saveGame.FileExists("user://savegame.save")) {
		//	StartIntro();
		//LoadingGame();
	        return; //Error!  We don't have a save to load.
	    }
		 
	    //We need to revert the game state so we're not cloning objects during loading.  This will vary wildly depending on the needs of a project, so take care with this step.
	    //For our example, we will accomplish this by deleting savable objects.
	    var saveNodes = GetTree().GetNodesInGroup("IPersist");
	    foreach (Node saveNode in saveNodes)
	        saveNode.QueueFree();
	
	    //Load the file line by line and process that dictionary to restore the object it represents
	    saveGame.Open("user://savegame.save", (int)File.ModeFlags.Read);
	
	    while (!saveGame.EofReached())
	    {
	        var currentLine = (Dictionary<object, object>)JSON.Parse(saveGame.GetLine()).Result;
	        if (currentLine == null)
	            continue;
	
	        //First we need to create the object and add it to the tree and set its position.
	        var newObjectScene = (PackedScene)ResourceLoader.Load(currentLine["Filename"].ToString());
	        var newObject = (Node)newObjectScene.Instance();
	        GetNode(currentLine["Parent"].ToString()).AddChild(newObject);
	        newObject.Set("Position", new Vector2((float)currentLine["PosX"], (float)currentLine["PosY"]));
	
	        //Now we set the remaining variables.
	        foreach (System.Collections.Generic.KeyValuePair<object, object> entry in currentLine)
	        {
	            string key = entry.Key.ToString();
	            if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY")
	                continue;
	            newObject.Set(key, entry.Value);
	        }
	    }
	
	    saveGame.Close();
	}
}