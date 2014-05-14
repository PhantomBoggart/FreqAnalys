﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace StatAnalys
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int[] original_statistic = new int[0x100];
            int[] encrypted_statistic = new int[0x100];
            int[,] original_sorted = new int[0x100,0x2];
            int[,] encrypted_sorted = new int[0x100, 0x2];
            byte[] encrypted;
            
            int encrypted_length = 0;
            /*for (int i = 0; i < 255; i++) { original_statistic[i] = 0;
                                            encrypted_statistic[i] = 0;
                                            };*/

            Console.Write("Choose file for enrypting\n");
            Console.ReadKey();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {

                /*StreamReader streamReader = new StreamReader(dlg.FileName); //Открываем файл для чтения
                string str = ""; //Объявляем переменную, в которую будем записывать текст из файла

                while (!streamReader.EndOfStream) //Цикл длиться пока не будет достигнут конец файла
                {
                    str += streamReader.ReadLine(); //В переменную str по строчно записываем содержимое файла
                }*/

                string str = File.ReadAllText(dlg.FileName);
                byte[] original = File.ReadAllBytes(dlg.FileName);

                int i_max = original.Length;
                for (int i = 0; i < i_max; i++)
                {
                    original_statistic[original[i]]++;
                };

                try
                {

                    //string str = "Here is some data to encrypt!";

                    // Create a new instance of the AesCryptoServiceProvider 
                    // class.  This generates a new key and initialization  
                    // vector (IV). 
                    using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                    {

                        // Encrypt the string to an array of bytes. 
                        encrypted = EncryptStringToBytes_Aes(str, myAes.Key, myAes.IV);
                        encrypted_length = encrypted.Length;

                        for (int i = 0; i < encrypted.Length; i++)
                        {
                            encrypted_statistic[encrypted[i]]++;
                        };

                        // Decrypt the bytes to a string.
                        string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                        //Display the original data and the decrypted data.
                        Console.WriteLine("Original:   {0}", str);
                        Console.WriteLine("Round Trip: {0}", roundtrip);
                        WaitForChangedResult.ReferenceEquals(null, str);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                }
            }

            for (int i = 0; i <= 0xFF; i++)
            {
                original_sorted[i, 0] = i;
                original_sorted[i, 1] = original_statistic[i];
                encrypted_sorted[i, 0] = i;
                encrypted_sorted[i, 1] = encrypted_statistic[i];
            }

            for (int i = 0; i <= 0xFF; i++) {
                for (int j = 0xFF; j >= i; j--) {
                    if (original_sorted[j, 1] > original_sorted[i, 1]) {
                        int a = original_sorted[i, 1], b = original_sorted[i, 0];
                        original_sorted[i, 1] = original_sorted[j, 1];
                        original_sorted[i, 0] = original_sorted[j, 0];
                        original_sorted[j, 1] = a;
                        original_sorted[j, 0] = b;
                       }
                    if (encrypted_sorted[j, 1] > encrypted_sorted[i, 1])
                    {
                        int a = encrypted_sorted[i, 1], b = encrypted_sorted[i, 0];
                        encrypted_sorted[i, 1] = encrypted_sorted[j, 1];
                        encrypted_sorted[i, 0] = encrypted_sorted[j, 0];
                        encrypted_sorted[j, 1] = a;
                        encrypted_sorted[j, 0] = b;
                    }


                }
            }
            /* byte[] encrypted_ = new Byte[encrypted_length];
            for (int i = 0; i < encrypted_length; i++) {

                for (int j = 0; j <= 0xFF; j++) {
                    if (encrypted_[i] == encrypted_sorted[j, 0]) { encrypted_[i]=}
                    }
            
            */


                Console.WriteLine("\nСтатистика по байтам в порядке возрастания\ni,Символ,Символы оригинала,Символы шифр. файла, \n");
            
            
            for (int i = 0x0; i <= 0xFF; i++)
            {
                if ((i > 0xF) || (i < 0x8))
                {
                    Console.WriteLine("{0:X2}| {1,2} | {2,4} | {3,4}  | {4:X2}:{5,4}  | {6:X2}:{7,4}  ", i, Convert.ToChar(i), original_statistic[i], encrypted_statistic[i], Convert.ToChar(encrypted_sorted[i, 0]), encrypted_sorted[i, 1],Convert.ToChar(original_sorted[i, 0]), original_sorted[i, 1]);
                }
                else
                {
                    Console.WriteLine("{0:X2}| {1:X3}| {2,4}  | {3,4} | {4:X2}:{5,4}  | {6:X2}:{7,4}  ", i, i, original_statistic[i], encrypted_statistic[i], Convert.ToChar(encrypted_sorted[i, 0]), encrypted_sorted[i, 1], Convert.ToChar(original_sorted[i, 0]), original_sorted[i, 1]);
                }
            }
            Console.WriteLine("\nНажмите Enter для продолжения\n");

            Console.ReadKey();





        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV. 
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}