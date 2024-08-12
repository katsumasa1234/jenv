using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace jenv
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("引数が不正です");
				showHelp();
			}
			else
			{
				switch (args[0])
				{
					case "version":
						showVersion();
						break;
					case "versions": 
						showVersions();
						break;
					case "help": 
						showHelp(); 
						break;
					case "change":
						try
						{
							changeVersion(args[1]);
						}
						catch
						{
							Console.WriteLine("引数が不正です");
							showHelp();
						}
						break;
					case "install":
						try
						{
							install(args[1]); 
						} catch
						{
							Console.WriteLine("引数が不正です");
							showHelp();
						}
						break;
					case "list":
						try
						{
							showCanInstallVersions(args[1]);
						}
						catch
						{
							showAvailableVersion();
						}
						break;
					case "delete":
						try
						{
							deleteVersion(args[1]);
						}
						catch
						{
							Console.WriteLine("引数が不正です");
							showHelp();
						}
						break;
					default: 
						Console.WriteLine("引数が不正です"); 
						showHelp(); 
						break;
				}
			}
		}

		static void showVersion()
		{
			var path = Environment.GetEnvironmentVariable("JAVA_HOME");
			Console.WriteLine(filename(path));
		}

		static void showHelp()
		{
			Console.WriteLine(
				"・help [ヘルプを表示します]\n" +
				"・version [現在使用中のバージョンを表示します]\n" +
				"・versions [ダウンロードされているバージョン一覧を表示します]\n" +
				"・change version [指定したバージョンに変更します] (例: jenv change jdk-20.0.2+9)\n" +
				"・list [利用可能なバージョンを表示します]\n" +
				"・list version [指定したバージョンのインストール可能なバージョンを表示します (例: jenv list 20)\n" +
				"・install version [指定したバージョンをインストールします] (例: jenv install jdk-20.0.2+9)\n" +
				"・delete version [指定したバージョンを削除します] (例: jenv delete jdk-20.0.2+9)");
		}

		static void changeVersion(string version)
		{
			var versions = getVersions();
			var versionPath = Environment.GetEnvironmentVariable("JENV_HOME") + "\\java\\" + version;
			if (Array.IndexOf(versions, versionPath) != -1)
			{
				Environment.SetEnvironmentVariable("JAVA_HOME", versionPath, EnvironmentVariableTarget.User);
				Console.WriteLine("変更が完了しました\n適用させるためにはプロセスを再起動してください");
			} else
			{
				Console.WriteLine("ダウンロードされていないバージョンです");
			}
		}

		static void showVersions()
		{
			var versions = getVersions();
			foreach (var version in  versions)
			{
				Console.WriteLine(filename(version));
			}
		}

		static string[] getVersions()
		{
			var directoryInfo = new DirectoryInfo(Environment.GetEnvironmentVariable("JENV_HOME") + "\\java");
			var versions = directoryInfo.GetDirectories();
			string[] versionsPath = new string[versions.Length];
			for (int i = 0; i < versions.Length; i++)
			{
				versionsPath[i] = versions[i].FullName;
			}
			return versionsPath;
		}

		static string filename(string path)
		{
			return Path.GetFileName(path);
		}

		static void install(string version)
		{
			string url = "https://api.adoptium.net/v3/binary/version/" + version + "/windows/x64/jdk/hotspot/normal/eclipse?project=jdk";
			using (HttpClient client = new HttpClient())
			{
				try
				{
					Console.WriteLine("ダウンロード中...");
					byte[] bytes = client.GetByteArrayAsync(url).Result;
					string outputPath = Environment.GetEnvironmentVariable("JENV_HOME");
					File.WriteAllBytes(outputPath + "/jdk.zip", bytes);
					Console.WriteLine("ダウンロードが完了しました\nファイルを展開しています...");
					ZipFile.ExtractToDirectory(outputPath + "/jdk.zip", outputPath + "/java/");
					Console.WriteLine("展開が完了しました\nキャッシュを削除しています...");
					File.Delete(outputPath + "/jdk.zip");
					Console.WriteLine("インストールが完了しました");
				} catch
				{
					Console.WriteLine("存在しないバージョンを指定しているか、ネットワーク通信に異常が発生しました");
				}
			}
		}

		static void showCanInstallVersions(string version)
		{
			var versions = getCanInstallVersions(version);
			foreach (var versionName in versions)
			{
				Console.WriteLine(versionName);
			}
		}

		static List<string> getCanInstallVersions(string version)
		{
			try
			{
				string apiUrl = "https://api.adoptium.net/v3/info/release_names?architecture=x64&heap_size=normal&image_type=jdk&jvm_impl=hotspot&page=0&os=windows&page_size=20&project=jdk&release_type=ga&semver=false&sort_method=DEFAULT&sort_order=DESC&vendor=eclipse&version=%5B" + version + "%2C" + (int.Parse(version) + 1) + "%29";
				List<string> versions = new List<string>();
				using (HttpClient client = new HttpClient())
				{
					string jsonResponse = client.GetStringAsync(apiUrl).Result;
					JArray json = JArray.Parse(JObject.Parse(jsonResponse)["releases"].ToString());
					for (int i = 0; json.Count > i; i++)
					{
						versions.Add(json[i].ToString());
					}
				}
				return versions;
			}
			catch
			{
				Console.WriteLine("指定したバージョンが不正です");
				return new List<string>();
			}
		}

		static void deleteVersion(string version)
		{
			try
			{
				string path = Environment.GetEnvironmentVariable("JENV_HOME") + "/java/" + version;
				Directory.Delete(path, true);
				Console.WriteLine("削除が完了しました");
			}
			catch
			{
				Console.WriteLine("削除に失敗、もしくは存在しないバージョンを指定しています");
			}
		}

		static void showAvailableVersion()
		{
			string url = "https://api.adoptium.net/v3/info/available_releases";
			using (HttpClient client = new HttpClient())
			{
				string result = client.GetStringAsync(url).Result;
				JArray list = JArray.Parse(JObject.Parse(result)["available_releases"].ToString());
				foreach (string item in list)
				{
					Console.WriteLine(item);
				}
			}
		}
	}
}
