using DeviceRunners.VisualRunners.Maui;

namespace icu.net.android.tests;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new VisualRunnerAppShell());
	}
}
