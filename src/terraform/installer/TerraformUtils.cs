using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace fountain {
  public static class TerraformUtils {

    public class TerraformBuild {
      public string arch {
        get;
        set;
      }
      public string filename {
        get;
        set;
      }
      public string name {
        get;
        set;
      }
      public string os {
        get;
        set;
      }
      public string url {
        get;
        set;
      }
      public string version {
        get;
        set;
      }

      public string ToString() => $"{arch}:{os}";
    }

    public class TerraformVersion {
      public TerraformBuild[] builds;
      public string name {
        get;
        set;
      }
      public string version {
        get;
        set;
      }
    }

    public class TerraformVersions {
      public string name {
        get;
        set;
      }
      public Dictionary < string, TerraformVersion > versions {
        get;
        set;
      }
    }

    public static async Task<Dictionary<string, string>> GetVersionsAsync() {
      using(WebClient client = new WebClient()) {
        string json = await client.DownloadStringTaskAsync("https://releases.hashicorp.com/terraform/index.json");
        TerraformVersions terraformVersions = JsonConvert.DeserializeObject<TerraformVersions>(json);

        // exclude all versions where "os = terraform" due to duplicate keys
        foreach (var version in terraformVersions.versions.ToList()) {
            version.Value.builds = version.Value.builds.Where(b => b.os != "terraform").ToArray();
            if (version.Value.builds.Length == 0) {
                terraformVersions.versions.Remove(version.Key);
            }
        }

        // complete
        return terraformVersions.versions
            .SelectMany(x => x.Value.builds, (v, b) => new { v, b })
            .ToDictionary(x => $"{x.v.Value.version}:{x.b.os}:{x.b.arch}", x => x.b.url);
      }
    }

    public static string ScriptPath
    {
      get {
      string scriptPath = Path.Combine(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "src"), "terraform"), "installer");
      string linuxMacFilePath = Path.Combine(scriptPath, "install.sh");
      string windowsFilePath = Path.Combine(scriptPath, "install.bat");
      scriptPath = windowsFilePath;

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
        // Linux or macOS
        scriptPath = linuxMacFilePath;
      }
      return scriptPath;
      }
    }

    public static async Task < bool > Install(string uri, string installPath = null, bool installIfExistant = false) {
      string scriptPath = ScriptPath;
      if (installPath == null) installPath = "C:/terraform";
      Console.WriteLine(installPath);
      if (!installIfExistant && Directory.Exists(installPath)) {
        Console.WriteLine("Terraform already exists at the given path - include option '--forceinstall' if this was a mistake. @" + scriptPath);
        return false;
      }

      // Check if the script file exists
      if (File.Exists(scriptPath)) {
        // Execute the script file
        Process process = new Process();
        process.StartInfo.FileName = scriptPath;
        process.StartInfo.Arguments = uri;
        process.Start();
        await process.WaitForExitAsync().ConfigureAwait(false);
      } else {
        Console.WriteLine("ERROR: Script file not found at " + scriptPath);
        return false;
      }
      return true;
    }
  }
}