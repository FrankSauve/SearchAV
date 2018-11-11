using System;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace RC_SpeechToText.Utils
{
    [Route("converter")]
    public class Converter
    {

        //TODO: ADD A METHOD THAT WILL SELECT THE METHOD DEPENDING ON WHICH TYPE IS THE FILE. IT WILL ALSO RETURN THE FILE IN IFORM FORMAT.

        //TODO: ADD A METHOD THAT WILL DELETE THE CONVERTED FILE FROM THE SERVER ONCE WE ARE DONE WITH IT (ONCE THE TRANSCRIPTION IS DONE). 

        public string VideoToAudio()
        {
            var inputLocation = "C:\\Users\\sarb\\Desktop\\rad\\RAD_Vegan.mp4";
            var outputLocation = "C:\\Users\\sarb\\Desktop\\rad\\RAD_Vegan.wav";

            var inputFile = new MediaFile { Filename = inputLocation };
            var outputFile = new MediaFile { Filename = outputLocation };

            var conversionOptions = new ConversionOptions
            {
                AudioSampleRate = AudioSampleRate.Hz22050
            };

            try
            {
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile, conversionOptions);
                }
            }
            catch (Exception ex)
            {
                return "Conversion Unsuccessful";
            }
            StereoToMono(outputLocation);
            return "Conversion successful. Output file at: " + outputLocation;
        }
        public String StereoToMono(String outputLocation)
        {
            var outputLocationMono = "C:\\Users\\sarb\\Desktop\\rad\\RAD_Vegan_Mono.wav";
            var outputWaveLocation = "C:\\Users\\sarb\\Desktop\\rad\\RAD_Vegan.wav";
            try
            {
                using (var inputReader = new AudioFileReader(outputLocation))
                {
                    var mono = new StereoToMonoSampleProvider(inputReader);
                    mono.LeftVolume = 0.0f;
                    mono.RightVolume = 1.0f;
                    WaveFileWriter.CreateWaveFile16(outputLocationMono, mono);
                }
            }
            catch (Exception ex)
            {
                return "Conversion Unsuccessful";
            }
            System.IO.File.Delete(outputWaveLocation);
            return "Conversion from stereo to mono successful.";
        }
    }
}
