using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class WavUtilityOptimized
{
    // New method that takes the actual recorded sample count.
    public static byte[] FromAudioClip(AudioClip clip, int recordedSamples)
    {
        if (clip == null)
        {
            Debug.LogError("WavUtilityOptimized: AudioClip is null.");
            return null;
        }
        
        int channels = clip.channels;
        int sampleCount = recordedSamples * channels; // Process only the recorded samples.
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        // Convert float samples [-1, 1] to 16-bit PCM samples.
        byte[] pcmData = new byte[sampleCount * 2]; // 2 bytes per sample.
        int offset = 0;
        for (int i = 0; i < sampleCount; i++)
        {
            // Clamp sample values to ensure they fit in 16-bit.
            short pcmSample = (short)Mathf.Clamp(samples[i] * short.MaxValue, short.MinValue, short.MaxValue);
            pcmData[offset++] = (byte)(pcmSample & 0xff);
            pcmData[offset++] = (byte)((pcmSample >> 8) & 0xff);
        }
        
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // RIFF header.
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + pcmData.Length);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            
            // fmt subchunk.
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // Subchunk1Size for PCM.
            writer.Write((short)1); // AudioFormat (1 = PCM).
            writer.Write((short)channels);
            writer.Write(clip.frequency);
            writer.Write(clip.frequency * channels * 2); // ByteRate.
            writer.Write((short)(channels * 2)); // BlockAlign.
            writer.Write((short)16); // Bits per sample.
            
            // data subchunk.
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(pcmData.Length);
            writer.Write(pcmData);
            
            return stream.ToArray();
        }
    }
}