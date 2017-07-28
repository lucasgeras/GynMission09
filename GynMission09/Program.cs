using System;
using System.IO;
using System.Collections;


namespace GynMission09
{
    class Program
    {
        public static byte[] rawdata;
        public static byte[] wavdata;
        public static BitArray messagebits;
        public static int channels;
        public static int Bitsforsample;
        public static int Rate;
        public static int Lenght;
        static void Main(string[] args)
        {
            Console.WriteLine("ACTION: Reading wav file. Please wait...");
            Console.WriteLine();
            ReadBytes();
            DecodeBits();
            Console.WriteLine("INPUT: Press any key to exit program.");
            Console.ReadKey();
        }
        static void ReadBytes()
        {
            try
            {
                //Load file header information
                rawdata = File.ReadAllBytes("rec.wav");
                channels = BitConverter.ToInt16(new byte[2] { rawdata[22], rawdata[23] }, 0); //22 23 opisuje ilość kanałów
                Bitsforsample = BitConverter.ToInt16(new byte[2] {rawdata[34],rawdata[35] }, 0);
                Rate = BitConverter.ToInt32(new byte[4] { rawdata[24], rawdata[25], rawdata[26], rawdata[27] }, 0);
                Lenght = BitConverter.ToInt32(new byte[4] { rawdata[40], rawdata[41], rawdata[42], rawdata[43] }, 0);
                Console.WriteLine("Channels: " + channels + "; ");
                Console.WriteLine("Bits per sample: " + Bitsforsample + "; ");
                Console.WriteLine("Sampling Rate: " + Rate + "; ");
                Console.WriteLine("Data bytes number: " + Lenght + "; ");
                Console.WriteLine("Duration: " + (double)(Lenght / Rate/2) + " seconds; ");
                Console.WriteLine();

                Int16 tempValue;
                wavdata = new byte[Lenght]; // tablica na dane dźwięku
                messagebits = new BitArray(Lenght/Rate/2);
                Array.Copy(rawdata, wavdata, Lenght);
                int Counter = 0;
                for(int i = Rate + 1; i < Lenght; i += 44100 * 2) // sprawdzamy wartość sampla w połowie sekundy, i wykonujemy skok o sekundę
                {
                    tempValue = BitConverter.ToInt16(new byte[2] { wavdata[i+1], wavdata[i]}, 0);
                    if (Math.Abs(tempValue) > 11000) messagebits[Counter] = false;
                    else messagebits[Counter] = true;
                    Counter++;
                    //Console.WriteLine(tempValue);
                }
                Console.WriteLine("ACTION: Reading Binary code. Please wait...");
                Console.WriteLine();
                for (int i = 0; i < messagebits.Length; i++)
                {
                    if (messagebits[i] == false) Console.Write("0");
                    else Console.Write("1");
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            catch(Exception e)
            {
                Console.Write("ERROR: Problem reading file. Exiting..." + Environment.NewLine + e.Message + Environment.NewLine);
            }
        }
        public static void DecodeBits()
        {
            Console.WriteLine("ACTION: Decoding Secret Message. Please wait...");
            Console.WriteLine();
            byte[] message = new byte[messagebits.Length / 8];
            char[] messagechars = new char[messagebits.Length/8];
            messagebits.CopyTo(message, 0);
            for(int i = 0; i < message.Length; i++)
            {
                messagechars[i] = (char)message[i];
            }
            Console.WriteLine(messagechars);
            Console.WriteLine();

        }

    }
}
