using Avalonia.Platform;
using NAudio.Wave;
using Serilog;
using ShadowPet.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
namespace ShadowPet.Desktop.Services
{
    public class AudioService : IDisposable
    {
        private readonly Random _random = new();
        private readonly SettingsService _settingsService;
        private readonly List<string> _speakSoundPaths =
        [
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_2.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_3.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_4.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_6.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_7.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_8.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_9.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_10.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_12.mp3",
            "avares://ShadowPet/Assets/Raw/Audio/WilsonVoice_generic_13.mp3"

        ];
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;

        public AudioService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void PlayRandomSpeakSound()
        {
            try
            {
                var soundPath = _speakSoundPaths[_random.Next(_speakSoundPaths.Count)];

                DisposeCurrentSound();

                _outputDevice = new WaveOutEvent();

                var assetStream = AssetLoader.Open(new Uri(soundPath));

                var memoryStream = new MemoryStream();
                assetStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var waveStream = new Mp3FileReader(memoryStream);
                _outputDevice.Init(waveStream);
                var settings = _settingsService.LoadSettings();
                _outputDevice.Volume = (float)(settings.SoundVolume / 100.0);
                _outputDevice.Play();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Falló la reproducción de audio");
            }
        }

        private void DisposeCurrentSound()
        {
            _outputDevice?.Dispose();
            _outputDevice = null;
            _audioFile?.Dispose();
            _audioFile = null;
        }

        public void Dispose()
        {
            DisposeCurrentSound();
            GC.SuppressFinalize(this);
        }
    }
}
