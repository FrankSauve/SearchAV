﻿using System;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Mvc;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;

namespace RC_SpeechToText.Utils
{
    public class Converter
    {
        //TODO: ADD A METHOD THAT WILL DELETE THE CONVERTED FILE FROM THE SERVER ONCE WE ARE DONE WITH IT (ONCE THE TRANSCRIPTION IS DONE). 

        public string FileToWav(string inputFilePath)
        {
            var wavFileLocation = inputFilePath.Substring(0, inputFilePath.LastIndexOf('.')+1) + "wav"; 

            var inputFile = new MediaFile { Filename = inputFilePath };
            var outputFile = new MediaFile { Filename = wavFileLocation };

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

				return StereoToMono(wavFileLocation);
			}
			catch
			{
				return null;
			}
        }
        public string StereoToMono(string wavFileLocation)
        {
            var monoFileLocation = wavFileLocation.Substring(0, wavFileLocation.LastIndexOf('.')) + "_Mono.wav";

            using (var inputReader = new AudioFileReader(wavFileLocation))
            {
                var mono = new StereoToMonoSampleProvider(inputReader);
                mono.LeftVolume = 0.0f;
                mono.RightVolume = 1.0f;
                WaveFileWriter.CreateWaveFile16(monoFileLocation, mono);
            }

            DeleteFile(wavFileLocation); 

            return monoFileLocation;
        }

        public void DeleteFile(string wavFilePath)
        {
            System.IO.File.Delete(wavFilePath);
        }

        public string CreateThumbnail(string videoFilePath, string outputFilePath)
        {
			try
			{
				using (var engine = new Engine())
				{
					var mp4 = new MediaFile { Filename = videoFilePath };
					var outputFile = new MediaFile { Filename = outputFilePath };

					var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(1) };

					engine.GetMetadata(mp4);

					engine.GetThumbnail(mp4, outputFile, options);

					return outputFile.Filename;
				}
			}
			catch
			{
				return null;
			}
        }
    }
}
