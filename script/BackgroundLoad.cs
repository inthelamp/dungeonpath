using Godot;
using System;
using System.Threading;

public class BackgroundLoad2 : Node
{
	private const int OK = 0;
	private const int ERR_FILE_EOF = 18;
	private const int SIMULATED_DELAY_SEC = 1;
	
	private System.Threading.Thread _thread = null;	
 	private ResourceInteractiveLoader _loader = null;
	private ProgressBar _progress = null;	
		
    public override void _Ready()
    {	   
		_progress = (ProgressBar)GetNode("ProgressBar");
	}

	public void _ThreadLoad(string path)
	{
	    _loader = ResourceLoader.LoadInteractive(path);
		
		//check for errors
	    if (_loader == null) 
		{
			GD.Print("ResourceLoader is null and check if the resource path is valid : ", path);
	        return;
		} 
		
		var total = _loader.GetStageCount();

		//Call deferred to configure max load steps
		CallDeferred(nameof(DeferredProgressBarSetMax), total);	
		
		PackedScene pScene = null;
		
		//#iterate until we have a resource
		//Update progress bar, use call deferred, which routes to main thread	
		while (true) 
		{
			//Call deferred to set load step
			CallDeferred(nameof(DeferredProgressBarSetValue), _loader.GetStage());	
			//Simulate a delay
			OS.DelayMsec( SIMULATED_DELAY_SEC * 1000);
		
			GD.Print("Value: ", _loader.GetStage());	
						
	        var err = (int) _loader.Poll();
		
			GD.Print("Error: ", err);
			
	       if (err == ERR_FILE_EOF) //load finished
			{			
	            pScene = (PackedScene)_loader.GetResource();
	            break;
	        } else if (err != OK) { 
				GD.Print("ResourceLoader failed on loading resource!!");
	            break;
			}
		}
		
		_loader = null;
		
		//Send whathever we did (or not) get
		CallDeferred(nameof(_ThreadDone), pScene);
	}
	
	public void _ThreadDone(PackedScene pScene)	
	{
		//check for errors
	    if (pScene == null) 
		{
			GD.Print("No resource is available.");
	        return;
		} 	

		GD.Print("Thread is done.");
			
		//_thread.WaitToFinish();
		_thread.Join();
		_progress.Hide();
		
		Node newScene = pScene.Instance();
		
		GetTree().CurrentScene.Free();
		GetTree().CurrentScene = null;
		
		GetTree().Root.AddChild(newScene);
		GetTree().CurrentScene = newScene;

		_progress.Visible = false;
	}
	
	public void DeferredProgressBarSetMax(int total)
	{	
		_progress.SetMax(total);
	}	
	
	public void DeferredProgressBarSetValue(int value)
	{	
		_progress.SetValue(value);
	}
	
	public void GotoScene(string path) 
	{
		CallDeferred(nameof(DeferredGotoScene), path);
	}	
	
	public void DeferredGotoScene(string path) 
	{
		_thread = new System.Threading.Thread(() => _ThreadLoad(path));
		_thread.Start();
		Raise(); //show on top			
		_progress.Visible = true;
	//	_progress.Show();
	}

}
