using Keepix.EvmPlugin.Input;
using Keepix.PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Keepix.EvmPlugin
{
    internal class EvmPlugin
    {
        private static BinaryObjectStore m_store;
        private static PluginState m_pluginState;

        public static void InitPlugin()
        {
            m_store = new BinaryObjectStore("data.bin");
            m_pluginState = PluginState.GetNewEmptyState();
        }

        private static void SaveState()
        {
            m_store.Store<PluginState>("state", m_pluginState);
        }

        [KeepixPluginFn("pre-install")]
        public static async Task<bool> OnPreInstallFunc()
        {
            return true;
        }

        [KeepixPluginFn("install-rocketpool-env")]
        public static async Task<bool> OnInstallRocketPoolEnv(InstallInput input)
        {
            // Init function call
            InitPlugin();
            SaveState();

            m_pluginState.State = PluginState.PLUGIN_STATE_INSTALL_DOCKER;
            SaveState();

            try
            {
                // Create a new folder to hold the CLI application
                ExecuteShellCommand("mkdir -p ~/bin");

                // Download the Rocket Pool CLI application
                ExecuteShellCommand("wget https://github.com/rocket-pool/smartnode-install/releases/latest/download/rocketpool-cli-linux-amd64 -O ~/bin/rocketpool");

                // Mark the downloaded file as executable
                var installResult = ExecuteShellCommand("chmod +x ~/bin/rocketpool && source ~/.bashrc && ~/bin/rocketpool service install");

                m_pluginState.State = PluginState.PLUGIN_STATE_IDLE;
                SaveState();

                if (installResult.Contains("was successfully installed"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [KeepixPluginFn("install")]
        public static async Task<bool> OnInstallFunc(InstallInput input)
        {

            return true;
        }

        [KeepixPluginFn("uninstall")]
        public static async Task<bool> OnUninstallFunc()
        {

            return true;
        }

        [KeepixPluginFn("plugin")]
        public static async Task<KeepixPluginInformation> OnPluginFunc()
        {
            return new KeepixPluginInformation
            {
                Id = "a60534c0-1f8d-4fa2-afae-50a37d29c794",
                Name = "Keepix.EvmPlugin",
                Description = "EVM Plugin for the Keepix Ecosystem",
                Version = "1.0.0-alpha"
            };
        }

        private static string ExecuteShellCommand(string command)
        {
            var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = new Process
            {
                StartInfo = processInfo
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Command '{command}' exited with code {process.ExitCode}");
            }
            return output;
        }
    }
}
