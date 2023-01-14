using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Newtonsoft.Json;

namespace fountain {
  /// <summary>
  /// Handles the conglomeration option for printing source code.
  /// </summary>
  public class TerraformCommand: CommandExt {
    /// <summary>
    /// Sets this commands handler and adds the following options:
    ///     install | installs the latest version or whatever version is specified by version
    ///     forceInstall | installs the latest terraform even if it exists, or the version specified by version
    ///     version | specifies the exact terraform version to use
    ///     versions | prints all recent terraform versions
    ///     installDir | defaults to working dir but this is the dir to use
    ///     cleanup | deletes Terraform if it was intalled to the specified dir
    /// </summary>
    /// <returns></returns>
    public TerraformCommand(): base("terraform") {
      // define options
      var installOption = new Option < bool >
        (
          name: "--install",
          description: "installs the latest version or whatever version is specified by version",
          getDefaultValue: () => false
        );
      var forceInstallOption = new Option < bool >
        (
          name: "--forceinstall",
          description: "installs the latest terraform even if it exists, or the version specified by version",
          getDefaultValue: () => false
        );
      var statusOption = new Option < bool >
        (
          name: "--status",
          description: "Returns the installation status and assocaited version, if possible",
          getDefaultValue: () => true
        );
      var versionOption = new Option < string >
        (
          name: "--version",
          description: "specifies the exact terraform version to use",
          getDefaultValue: () => "0.14.8"
        );
      var versionsOption = new Option < bool >
        (
          name: "--versions",
          description: "Prints out all possible versions, based on your OS.",
          getDefaultValue: () => false
        );
      var installDirOption = new Option < string >
        (
          name: "--installdir",
          description: "Directory that the source code will be read from",
          getDefaultValue: () => Path.Combine(FileUtils.GetFirstDrive(), "terraform")
        );
      var cleanupOption = new Option < bool >
        (
          name: "--cleanup",
          description: "deletes Terraform if it was intalled to the specified dir",
          getDefaultValue: () => false
        );

      // add options
      Add(installOption);
      Add(forceInstallOption);
      Add(statusOption);
      Add(versionOption);
      Add(versionsOption);
      Add(installDirOption);
      Add(cleanupOption);

      // set our handler
      this.SetHandler(
        async (install, forceInstall, info, version, versions, installDir, cleanup) => {
            // adhere to info by printing status and associated versions
            if (info) {
              if (!Directory.Exists(installDir)) {
                Console.WriteLine("STATUS: Uninstalled | directory does not exist. Run with --install");
              } else {
                string workingDirectory = TerraformUtils.ScriptPath;
                bool isCommandRecognized = true;

                try {
                  // Start the process
                  Process process = new Process();
                  process.StartInfo.WorkingDirectory = installDir;
                  process.StartInfo.UseShellExecute = true;
                  process.StartInfo.FileName = "terraform";
                  process.Start();

                  // Wait for the process to exit
                  process.WaitForExit();
                } catch (Exception exception) {
                  isCommandRecognized = false;
                }
                Console.WriteLine("STATUS: " + (isCommandRecognized ? "Installed" : "Uninstalled | directory exists but cmd is unreconizable. Try using the --forceinstall option, or --cleanup and then --install."));
              }
            }
            // check for confliction between install, forceInstall and cleanup
            if ((install || forceInstall) && cleanup) {
              // debug
              install = false;
              forceInstall = false;

              Console.WriteLine("--forceinstall or --install cannot be used with --cleanup.\nDisabled the formers");
              //return;
            }

            // check for confliction between install and forceInstall, but allow it
            if (install && forceInstall) {
              install = false;
              Console.WriteLine("--forceinstall and --install are used together, the former is adhered to.");
            }

            // check for versions option to print
            if (versions != false) {
              Console.WriteLine("Querying versions...");
              var dict = await TerraformUtils.GetVersionsAsync();
              Console.WriteLine("-------------------------------------");
              foreach(KeyValuePair < string, string > pair in dict) {
                Console.WriteLine($"({pair.Key.ToString()})");
              }
            }

            // setup uri by checking version validity and choosing the appropriate
            // runtime and os
            string uri = null;
            if (install || forceInstall) {
              var dict = await TerraformUtils.GetVersionsAsync();
              bool has = false;
              foreach(KeyValuePair < string, string > pair in dict) {
                string[] str = pair.Key.Split(":");
                string _version = str[0], os = str[1], arch = str[2];
                if (version == _version) {
                  has = true;
                  uri = pair.Value;
                  Console.WriteLine("OS = " + os + ", ARCH=" + arch);
                  //continue;
                }
              }
              if (!has) {
                Console.WriteLine("Version string is not valid. Run with the option --versions to see all valid versions.");
                return;
              }
            }

            // ensure install dir is valid, and if it is, install if Terraform doesnt exist (if --install)
            if (install) {
              StringUtils.PrintMessage = "Installing...";
              StringUtils.StartPrinting(1000);
              bool resp = await TerraformUtils.Install(uri, installDir, false);
              StringUtils.StopPrinting();
              Console.WriteLine("Installed " + (resp ? "" : "not ") + "succesfully installed to " + installDir + "!");
            } else if (forceInstall) {
              // ensure install dir is valid, and if it is, install Terraform (--forceinstall)
              Console.WriteLine("Installing...");
              bool resp = await TerraformUtils.Install(uri, installDir, true);
              Console.WriteLine("Installed " + (resp ? "" : "not ") + "succesfully installed to " + installDir + "!");
            } else if (cleanup) {
              // adhere to cleanup
              string directoryPath = installDir;

              try {
                // Delete the directory and its contents recursively
                foreach(string entry in Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.AllDirectories)) {
                  if (File.Exists(entry)) {
                    File.Delete(entry);
                  } else {
                    Directory.Delete(entry, true);
                  }
                }
                Directory.Delete(directoryPath, true);
                Console.WriteLine("Directory deleted successfully.");
              } catch (IOException ex) {
                Console.WriteLine("Error deleting directory: " + ex.Message);
              }
              Console.WriteLine("Cleanedup succesfully, " + installDir + "!");
            }
          },
          installOption, forceInstallOption, statusOption, versionOption, versionsOption, installDirOption, cleanupOption);
    }

  }
}