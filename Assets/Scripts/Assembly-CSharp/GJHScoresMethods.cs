public class GJHScoresMethods
{
	private GJHScoresWindow window;

	public GJHScoresMethods()
	{
		window = new GJHScoresWindow();
	}

	~GJHScoresMethods()
	{
		window = null;
	}

	public void ShowLeaderboards()
	{
		window.Show();
	}

	public void DismissLeaderboards()
	{
		window.Dismiss();
	}
}
