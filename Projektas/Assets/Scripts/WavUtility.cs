using UnityEngine;
using System;
using System.Text;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("WavUtility: AudioClip is null.");
            return null;
        }

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * short.MaxValue);
        }

        int bytesPerSample = 2;
        int subChunk2Size = intData.Length * bytesPerSample;
        int fileSize = 44 + subChunk2Size;

        byte[] wavBytes = new byte[fileSize];

        WriteBytes(wavBytes, 0, Encoding.UTF8.GetBytes("RIFF"));
        BitConverter.GetBytes(fileSize - 8).CopyTo(wavBytes, 4);
        WriteBytes(wavBytes, 8, Encoding.UTF8.GetBytes("WAVE"));
        WriteBytes(wavBytes, 12, Encoding.UTF8.GetBytes("fmt "));
        BitConverter.GetBytes(16).CopyTo(wavBytes, 16);
        BitConverter.GetBytes((short)1).CopyTo(wavBytes, 20);
        BitConverter.GetBytes((short)clip.channels).CopyTo(wavBytes, 22);
        BitConverter.GetBytes(clip.frequency).CopyTo(wavBytes, 24);
        BitConverter.GetBytes(clip.frequency * clip.channels * bytesPerSample).CopyTo(wavBytes, 28);
        BitConverter.GetBytes((short)(clip.channels * bytesPerSample)).CopyTo(wavBytes, 32);
        BitConverter.GetBytes((short)(bytesPerSample * 8)).CopyTo(wavBytes, 34);
        WriteBytes(wavBytes, 36, Encoding.UTF8.GetBytes("data"));
        BitConverter.GetBytes(subChunk2Size).CopyTo(wavBytes, 40);

        int offset = 44;
        for (int i = 0; i < intData.Length; i++)
        {
            BitConverter.GetBytes(intData[i]).CopyTo(wavBytes, offset);
            offset += 2;
        }

        return wavBytes;
    }

    private static void WriteBytes(byte[] buffer, int offset, byte[] bytesToWrite)
    {
        for (int i = 0; i < bytesToWrite.Length; i++)
        {
            buffer[offset + i] = bytesToWrite[i];
        }
    }
}
