using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Quinn.Editor
{
	public static class BuildManager
	{
		[MenuItem("Tools/Build All")]
		public static void BuildAll()
		{
			BuildGame(PlatformTarget.Windows);
			// TODO: Fix issues with mac, and possibly linux verions, not adhereing to their given paths.
		}

		private static void BuildGame(PlatformTarget target)
		{
			var levels = new string[] { "Assets/Scenes/Game.unity" };
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"/TinyWizard_{target}";

			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}

			var buildTarget = target switch
			{
				PlatformTarget.Windows => BuildTarget.StandaloneWindows,
				PlatformTarget.Mac => BuildTarget.StandaloneOSX,
				PlatformTarget.Linux => BuildTarget.StandaloneLinux64,
				_ => throw new Exception("Platform not supported!")
			};

			var report = BuildPipeline.BuildPlayer(levels, path + "/TinyWizard.exe", buildTarget, BuildOptions.None);

			UnityEngine.Debug.Log($"<bold>Build Finished. Result: <color=white>{report.summary.result}</color>!</bold>");

			string deletePath = path + "/TinyWizard_BurstDebugInformation_DoNotShip";
			if (Directory.Exists(deletePath))
			{
				Directory.Delete(deletePath, true);
			}

			string plat = target switch
			{
				PlatformTarget.Windows => "win",
				PlatformTarget.Mac => "mac",
				PlatformTarget.Linux => "linux",
				_ => throw new Exception("Platform not supported!")
			};
			var info = new ProcessStartInfo("butler.exe", $"push {path} quinncg/tiny-wizard:{plat}")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			var process = Process.Start(info);
			var stream = process.StandardOutput;

			while (stream.Peek() >= 0)
			{
				UnityEngine.Debug.Log(stream.ReadLine());
			}

			stream = process.StandardError;

			if (stream.Peek() < 0)
			{
				UnityEngine.Debug.Log($"<bold>Upload <color=green>Succeeded</color>!</bold>");
			}
			else
			{
				UnityEngine.Debug.Log($"<bold>Upload <color=red>Failed</color>!</bold>");
			}

			while (stream.Peek() >= 0)
			{
				UnityEngine.Debug.LogError(stream.ReadLine());
			}

			Directory.Delete(path);
		}
	}
}
