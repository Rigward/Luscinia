using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioCloud
{
    class _volumeController
    {
        public float Volume;

        public _volumeController()
        {
            try
            {
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);

                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    try
                    {
                        System.Diagnostics.Debug.Print(dev.FriendlyName);
                        Volume = dev.AudioEndpointVolume.MasterVolumeLevelScalar;
                        break;
                    }
                    catch { }
                }

            }
            catch { }
        }

        public void setSystemVolume(float vol)
        {
            try
            {
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);

                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    try
                    {
                        System.Diagnostics.Debug.Print(dev.FriendlyName);
                        dev.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
                        break;
                    }
                    catch { }
                }

            }
            catch { }
        }
    }
}
