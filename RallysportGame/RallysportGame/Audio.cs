using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

using System.Collections.Generic;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK;

namespace RallysportGame
{
    class Audio
    {

#if DEBUG 
        static readonly string filepath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()).ToString(),"Audio");//Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()).ToString(), @"Audio/SM64_The_Alternate_Route.wav");
#else
        static readonly string filepath = Path.Combine("Data", "Audio");//Path.Combine(Path.Combine("Data", "Audio"), "SM64_The_Alternate_Route.wav");
#endif
        static float gain = 0.1f;
        static int index = 0;
        static string[] audioFiles;

        static Dictionary<int, int> sourceToBuffer;
        static Audio()
        {
            sourceToBuffer = new Dictionary<int,int>();
            try
            {
                AudioContext AC = new AudioContext();
            }
            catch (AudioException ex)
            { // problem with Device or Context, cannot continue
                throw new System.ArgumentException("Issue with audio drivers");
            }

            audioFiles = Directory.GetFiles(filepath, "*.wav");
        }


        // Loads a wave/riff audio file.
        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
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

        private static ALFormat GetSoundFormat(int channels, int bits)
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
        public static int generateBS()
        {
            int[] output = new int[2];

            output[0] = AL.GenBuffer();
            output[1] = AL.GenSource();

            sourceToBuffer.Add(output[1], output[0]);
            
            return output[1];

        }

        /// <summary>
        /// Deletes a Selected Buffer and Source
        /// </summary>
        /// <param name="source"></param>
        public static void deleteBS(int source)
        {
            int buffer;
            if (audioStatus(source) != 3)
                stopSound(source);

            AL.DeleteSource(source);            
            sourceToBuffer.TryGetValue(source, out buffer);
            AL.DeleteBuffer(buffer);
            sourceToBuffer.Remove(source);

        }

        /// <summary>
        /// Loads a sound into selected Buffer and Source
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="source"></param>
        public static void loadSound(int source,int index)
        {
            string filename = audioFiles[index];

            int buffer;
            sourceToBuffer.TryGetValue(source, out buffer);
            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);
            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourcef.Gain, gain);
        }
        
        /// <summary>
        /// generates buffers, loads the sound
        /// </summary>
        /// <returns>the source thats used for the sound</returns>
        public static int initSound()
        {
            int source = generateBS();

            Audio.loadSound(source, index);

            return source;

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

        /// <summary>
        /// An easier way of keeping track of sources outside of the Audio class
        /// </summary>
        /// <param name="source"></param>
        /// <returns>value 0 = Initial, 1 = Playing, 2 = Paused, 3 = Stopped</returns>
        public static int audioStatus(int source)
        {
            int stateValue = 0;
            if (AL.GetSourceState(source) == ALSourceState.Playing)
                stateValue = 1;
            else if (AL.GetSourceState(source) == ALSourceState.Paused)
                stateValue = 2;
            else if (AL.GetSourceState(source) == ALSourceState.Stopped)
                stateValue = 3;


            return stateValue;
        }

        public static int nextTrack(int source)
        {
            deleteBS(source);

            source = generateBS();

            index++;
            if(index == audioFiles.Length)
                index = 0;
            
            loadSound(source,index);

            playSound(source);

            return source;
        }
        /// <summary>
        /// The source must be a mono source for this to have any effect
        /// Sets up the Position of the source in the world
        /// </summary>
        /// <param name="source"></param>
        /// <param name="position"></param>
        public static void setUpSourcePos(int source,Vector3 position)
        {
            AL.Source(source, ALSource3f.Position, ref position);
        }

        /// <summary>
        /// Sets the listeners position in the world, not that these values should be passed by referece
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="up"></param>
        public static void setUpListener(ref Vector3 position,ref Vector3 direction,ref Vector3 up)
        {
            AL.Listener(ALListener3f.Position, ref position);
            AL.Listener(ALListenerfv.Orientation, ref direction, ref up);
        }

        /// <summary>
        /// Gets the current position of the listener for debugging purpouses
        /// </summary>
        /// <returns></returns>
        public static Vector3 listenerPos()
        {
            float x,y,z;
            AL.GetListener(ALListener3f.Position,out x,out y,out z);
            return new Vector3(x, y, z);
        }
    }
}
