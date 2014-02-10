using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace RallysportGame
{
    class Audio
    {

#if DEBUG 
        static readonly string filename = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()).ToString(), @"Audio/SM64_The_Alternate_Route.wav");
#else
        static readonly string filename = Path.Combine(Path.Combine("Data", "Audio"), "SM64_The_Alternate_Route.wav");
#endif
        static float gain = 0.1f;

        public Audio()
        {
            
        }

        // Loads a wave/riff audio file.
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
        /// <summary>
        /// Generates buffer and source from openAL
        /// </summary>
        /// <returns>first element is the buffer index second is the source index</returns>
        public static int[] generateBS()
        {
            int[] output = new int[2];

            output[0] = AL.GenBuffer();
            output[1] = AL.GenSource();

            return output;

        }

        /// <summary>
        /// Deletes a Selected Buffer and Source
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="source"></param>
        public static void deleteBS(int buffer, int source)
        {

            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);

        }

        /// <summary>
        /// Loads a sound into selected Buffer and Source
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="source"></param>
        public static void loadSound(int buffer, int source)
        {

            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourcef.Gain, gain);
        }

        /// <summary>
        /// Stops the sound source and deletes it also deletes the buffer selected
        /// </summary>
        /// <param name="source"></param>
        public static void stopSound(int source)
        {
            AL.SourceStop(source);
        }
        /// <summary>
        /// Plays a specific source
        /// </summary>
        /// <param name="source"></param>
        public static void playSound(int source)
        {
            AL.SourcePlay(source);
        }

        /// <summary>
        /// Pauses a specific source
        /// </summary>
        /// <param name="source"></param>
        public static void pauseSound(int source)
        {
            AL.SourcePause(source);
        }

        /// <summary>
        /// Increases the volume of the selected source by 10%
        /// </summary>
        /// <param name="source">Audio source</param>
        public static void increaseGain(int source)
        {
            if (gain < 1.0f)
            {
                gain += 0.1f;
                AL.Source(source, ALSourcef.Gain, gain);
            }
        }

        /// <summary>
        /// Lowers the volume of the selected source by 10%
        /// </summary>
        /// <param name="source">Audio source</param>
        public static void decreaseGain(int source)
        {
            if (gain > 0.0f)
            {
                gain -= 0.1f;
                AL.Source(source, ALSourcef.Gain, gain);
            }
        }
    }
}
