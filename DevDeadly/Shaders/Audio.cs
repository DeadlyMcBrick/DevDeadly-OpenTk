using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK.Audio.OpenAL;

public class AudioPlayer : IDisposable
{
    private static ALDevice _device;
    private static ALContext _context;
    private static bool _initialized = false;

    private int _buffer;
    private int _source;

    public AudioPlayer(string filePath)
    {
        if (!_initialized)
        {
            _device = ALC.OpenDevice(null);
            if (_device == ALDevice.Null)
                throw new Exception("Can't open the Audio Device.");

            _context = ALC.CreateContext(_device, new ALContextAttributes());
            if (_context == ALContext.Null)
                throw new Exception("Can't create the audio.");

            ALC.MakeContextCurrent(_context);
            _initialized = true;
        }

        _buffer = AL.GenBuffer();
        _source = AL.GenSource();


        LoadWave(filePath, out byte[] soundData, out ALFormat format, out int freq);

        GCHandle handle = GCHandle.Alloc(soundData, GCHandleType.Pinned);
        try

        {
            IntPtr ptr = handle.AddrOfPinnedObject();
            AL.BufferData(_buffer, format, ptr, soundData.Length, freq);
        }

        finally
        {
            handle.Free();
        }

        AL.Source(_source, ALSourcei.Buffer, _buffer);
    }

    public void Play()
    {
        AL.SourcePlay(_source);
    }

    public void SetVolumen(float Volume)
    {
        AL.Source(_source, ALSourcef.Gain, Volume);
    }

    public void Dispose()
    {
        AL.SourceStop(_source);
        AL.DeleteSource(_source);
        AL.DeleteBuffer(_buffer);
    }
    public static void ShutdownAudio()
    {
        ALC.MakeContextCurrent(ALContext.Null);
        ALC.DestroyContext(_context);
        ALC.CloseDevice(_device);
        _initialized = false;
    }

    private void LoadWave(string filePath, out byte[] data, out ALFormat format, out int sampleRate)
    {
        using var reader = new BinaryReader(File.Open(filePath, FileMode.Open));

        reader.ReadChars(4); // "RIFF"
        reader.ReadInt32();  // File size
        reader.ReadChars(4); // "WAVE"
        reader.ReadChars(4); // "fmt "
        int fmtSize = reader.ReadInt32();
        short audioFormat = reader.ReadInt16();
        short numChannels = reader.ReadInt16();
        sampleRate = reader.ReadInt32();
        reader.ReadInt32(); // byteRate
        reader.ReadInt16(); // blockAlign
        short bitsPerSample = reader.ReadInt16();

        reader.BaseStream.Seek(fmtSize - 16, SeekOrigin.Current); // Skip extra bytes

        string dataChunkId = new(reader.ReadChars(4));
        while (dataChunkId != "data")
        {
            int chunkSize = reader.ReadInt32();
            reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
            dataChunkId = new(reader.ReadChars(4));
        }

        int dataSize = reader.ReadInt32();
        data = reader.ReadBytes(dataSize);

        format = (numChannels, bitsPerSample) switch
        {
            (1, 8) => ALFormat.Mono8,
            (1, 16) => ALFormat.Mono16,
            (2, 8) => ALFormat.Stereo8,
            (2, 16) => ALFormat.Stereo16,
            _ => throw new NotSupportedException("WAV Format not supported idk.")
        };
    }
}
