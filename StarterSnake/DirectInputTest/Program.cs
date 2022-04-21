using SharpDX.DirectInput;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirectInputTest
{
    class Program
    {
        private static readonly HttpClient client = new();

        private const string uri = "https://df-battlesnake-sillyboi20220319162850.azurewebsites.net/sifl/snake/input";
        //private const string uri = "http://localhost:17832/sifl/snake/input";

        //// Y: up = 0; none = 32511; down = 65535
        //// X: left = 0; none = 32511; right = 65535
        const int AxisMin = 0;
        const int AxisMid = 32511;
        const int AxisMax = 65535;

        static async Task<int> Main(string[] args)
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                        DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick/Gamepad found.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);

            Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                {
                    Console.WriteLine(state);

                    // Parse direction
                    if (state.Value == AxisMid)
                        continue;

                    var direction = string.Empty;
                    if (state.Offset == JoystickOffset.X)
                    {
                        direction = state.Value == AxisMin
                            ? "Left"
                            : "Right";
                    }
                    else if (state.Offset == JoystickOffset.Y)
                    {
                        direction = state.Value == AxisMin
                            ? "Up"
                            : "Down";
                    }

                    // Send Joystick info to API if specified
                    if (string.IsNullOrWhiteSpace(direction))
                        continue;

                    //var requestContent = new StringContent($"{{'direction': '{direction}'}}", Encoding.UTF8, "application/json");
                    var requestContent = new StringContent(direction, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{uri}?direction={direction}", requestContent);
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }
}
